using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using Common.Wrappers;
using Microsoft.Extensions.Options;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	// Every sign-in path (password, external provider) ends in IssueTokensAsync: clients only ever see our own tokens.
	public class AuthService(
		IUserRepository userRepository,
		IUserIdentityRepository identityRepository,
		IRefreshTokenRepository refreshTokenRepository,
		IVerificationCodeService verificationCodeService,
		IExternalAuthProviderRegistry externalProviders,
		IPasswordHasher passwordHasher,
		ITokenService tokenService,
		IOptions<AuthOptions> options) : IAuthService
	{
		private const string InvalidCredentials = "Invalid login or password.";
		private const string SessionExpired = "Your session has expired. Please sign in again.";
		private const string AccountBlocked = "This account is blocked.";
		private const string DefaultDisplayName = "User";

		private readonly AuthOptions _options = options.Value;

		public async Task<Result> SendCodeAsync(SendVerificationCodeRequest request)
		{
			if (request.Purpose == VerificationPurpose.LinkIdentity)
			{
				return Result.Failure("Linking codes are requested from the account endpoint.", ResultErrorKind.Validation);
			}

			if (!verificationCodeService.IsChannelSupported(request.Channel))
			{
				return Result.Failure($"Codes cannot be sent via '{request.Channel}'.", ResultErrorKind.Validation);
			}

			if (!LoginNormalizer.TryNormalize(request.Channel, request.Target, out var target))
			{
				return AuthValidation.InvalidTarget(request.Channel);
			}

			var exists = await identityRepository.ExistsAsync(request.Channel, target);

			if (request.Purpose == VerificationPurpose.Register && exists)
			{
				return Result.Failure($"An account with this {request.Channel} already exists.", ResultErrorKind.Conflict);
			}

			// Answer "sent" for unknown accounts so the reset form cannot be used to probe who is registered.
			if (request.Purpose == VerificationPurpose.ResetPassword && !exists)
			{
				return Result.Success();
			}

			return await verificationCodeService.SendAsync(request.Channel, target, request.Purpose);
		}

		public async Task<Result<AuthTokens>> RegisterAsync(RegisterRequest request)
		{
			var validation = ValidateAll(
				AuthValidation.ValidateClient(request.Client),
				AuthValidation.ValidateDisplayName(request.DisplayName),
				AuthValidation.ValidatePassword(request.Password)
			);

			if (!validation.Successful)
			{
				return Result<AuthTokens>.Failure(validation);
			}

			if (!LoginNormalizer.TryNormalize(request.Channel, request.Target, out var target))
			{
				return Result<AuthTokens>.Failure(AuthValidation.InvalidTarget(request.Channel));
			}

			var codeCheck = await verificationCodeService.VerifyAsync(request.Channel, target, VerificationPurpose.Register, request.Code);

			if (!codeCheck.Successful)
			{
				return Result<AuthTokens>.Failure(codeCheck);
			}

			var displayName = request.DisplayName.Trim();
			var passwordHash = passwordHasher.Hash(request.Password);
			int userId;

			try
			{
				userId = await userRepository.CreateWithIdentityAsync(displayName, passwordHash, request.Channel, target);
			}
			catch (DuplicateKeyException)
			{
				return Result<AuthTokens>.Failure($"An account with this {request.Channel} already exists.", ResultErrorKind.Conflict);
			}

			var tokens = await IssueTokensAsync(new UserModel(userId, displayName), request.Client);
			return Result<AuthTokens>.Success(tokens);
		}

		public async Task<Result<AuthTokens>> LoginAsync(PasswordLoginRequest request)
		{
			var clientCheck = AuthValidation.ValidateClient(request.Client);

			if (!clientCheck.Successful)
			{
				return Result<AuthTokens>.Failure(clientCheck);
			}

			if (!LoginNormalizer.TryResolveLogin(request.Login, out var provider, out var providerKey))
			{
				return Result<AuthTokens>.Failure(InvalidCredentials, ResultErrorKind.Unauthorized);
			}

			var credentials = await userRepository.GetCredentialsByIdentityAsync(provider, providerKey);

			if (credentials?.PasswordHash is null)
			{
				return Result<AuthTokens>.Failure(InvalidCredentials, ResultErrorKind.Unauthorized);
			}

			var check = passwordHasher.Verify(credentials.PasswordHash, request.Password ?? string.Empty);

			if (check == PasswordCheck.Failed)
			{
				return Result<AuthTokens>.Failure(InvalidCredentials, ResultErrorKind.Unauthorized);
			}

			if (credentials.IsBlocked)
			{
				return Result<AuthTokens>.Failure(AccountBlocked, ResultErrorKind.Unauthorized);
			}

			if (check == PasswordCheck.SuccessRehashNeeded)
			{
				await userRepository.UpdatePasswordHashAsync(credentials.UserId, passwordHasher.Hash(request.Password));
			}

			await identityRepository.TouchLastLoginAsync(provider, providerKey);

			var tokens = await IssueTokensAsync(new UserModel(credentials.UserId, credentials.DisplayName), request.Client);
			return Result<AuthTokens>.Success(tokens);
		}

		public async Task<Result<AuthTokens>> LoginExternalAsync(ExternalLoginRequest request)
		{
			var clientCheck = AuthValidation.ValidateClient(request.Client);

			if (!clientCheck.Successful)
			{
				return Result<AuthTokens>.Failure(clientCheck);
			}

			var provider = externalProviders.Find(request.Provider);

			if (provider is null)
			{
				return Result<AuthTokens>.Failure($"Sign-in via '{request.Provider}' is not supported.", ResultErrorKind.Validation);
			}

			var validation = await provider.ValidateAsync(request.Credential);

			if (!validation.Successful)
			{
				return Result<AuthTokens>.Failure(validation);
			}

			var identity = validation.Value;
			var credentials = await GetOrCreateExternalUserAsync(identity, request.DisplayNameHint);

			if (credentials.IsBlocked)
			{
				return Result<AuthTokens>.Failure(AccountBlocked, ResultErrorKind.Unauthorized);
			}

			var tokens = await IssueTokensAsync(new UserModel(credentials.UserId, credentials.DisplayName), request.Client);
			return Result<AuthTokens>.Success(tokens);
		}

		public async Task<Result<AuthTokens>> RefreshAsync(string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
			{
				return Result<AuthTokens>.Failure(SessionExpired, ResultErrorKind.Unauthorized);
			}

			var stored = await refreshTokenRepository.GetByHashAsync(HashRefreshToken(refreshToken));

			if (stored is null)
			{
				return Result<AuthTokens>.Failure(SessionExpired, ResultErrorKind.Unauthorized);
			}

			if (stored.RevokedAt is not null)
			{
				// A rotated token presented again means it leaked: kill every session of this user.
				if (stored.ReplacedById is not null)
				{
					await refreshTokenRepository.RevokeAllForUserAsync(stored.UserId);
				}

				return Result<AuthTokens>.Failure(SessionExpired, ResultErrorKind.Unauthorized);
			}

			var now = DateTime.UtcNow;

			if (stored.ExpiresAt <= now)
			{
				return Result<AuthTokens>.Failure(SessionExpired, ResultErrorKind.Unauthorized);
			}

			var credentials = await userRepository.GetCredentialsByIdAsync(stored.UserId);

			if (credentials is null || credentials.IsBlocked)
			{
				await refreshTokenRepository.RevokeAsync(stored.Id);
				return Result<AuthTokens>.Failure(SessionExpired, ResultErrorKind.Unauthorized);
			}

			var (newToken, newHash) = GenerateRefreshToken();
			var expiresAt = now + _options.RefreshTokenLifetime;

			var replacement = new NewRefreshToken(
				stored.UserId,
				newHash,
				stored.ClientType,
				stored.DeviceName,
				now,
				expiresAt
			);

			if (!await refreshTokenRepository.RotateAsync(stored.Id, replacement))
			{
				return Result<AuthTokens>.Failure(SessionExpired, ResultErrorKind.Unauthorized);
			}

			var user = new UserModel(credentials.UserId, credentials.DisplayName);
			var accessToken = tokenService.CreateAccessToken(user);

			var tokens = new AuthTokens(accessToken.Value, accessToken.ExpiresAt, newToken, expiresAt, user);
			return Result<AuthTokens>.Success(tokens);
		}

		public async Task LogoutAsync(string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken)) return;

			var stored = await refreshTokenRepository.GetByHashAsync(HashRefreshToken(refreshToken));

			if (stored is not null)
			{
				await refreshTokenRepository.RevokeAsync(stored.Id);
			}
		}

		public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
		{
			var passwordCheck = AuthValidation.ValidatePassword(request.NewPassword);

			if (!passwordCheck.Successful)
			{
				return passwordCheck;
			}

			if (!LoginNormalizer.TryNormalize(request.Channel, request.Target, out var target))
			{
				return AuthValidation.InvalidTarget(request.Channel);
			}

			var codeCheck = await verificationCodeService.VerifyAsync(request.Channel, target, VerificationPurpose.ResetPassword, request.Code);

			if (!codeCheck.Successful)
			{
				return codeCheck;
			}

			var credentials = await userRepository.GetCredentialsByIdentityAsync(request.Channel, target);

			if (credentials is null)
			{
				return Result.Failure("The code is invalid or has expired.", ResultErrorKind.Validation);
			}

			await userRepository.UpdatePasswordHashAsync(credentials.UserId, passwordHasher.Hash(request.NewPassword));
			await refreshTokenRepository.RevokeAllForUserAsync(credentials.UserId);

			return Result.Success();
		}

		public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
		{
			var passwordCheck = AuthValidation.ValidatePassword(request.NewPassword);

			if (!passwordCheck.Successful)
			{
				return passwordCheck;
			}

			var credentials = await userRepository.GetCredentialsByIdAsync(request.UserId);

			if (credentials is null)
			{
				return Result.Failure($"User {request.UserId} was not found.", ResultErrorKind.NotFound);
			}

			// Accounts created through an external provider may have no password yet: they set one without a current password.
			// Validation (not Unauthorized) on purpose: a 401 would make clients treat their access token as expired.
			if (credentials.PasswordHash is not null
				&& passwordHasher.Verify(credentials.PasswordHash, request.CurrentPassword ?? string.Empty) == PasswordCheck.Failed)
			{
				return Result.Failure("Current password is incorrect.", ResultErrorKind.Validation);
			}

			await userRepository.UpdatePasswordHashAsync(request.UserId, passwordHasher.Hash(request.NewPassword));

			var keepTokenHash = string.IsNullOrWhiteSpace(request.CurrentRefreshToken)
				? null
				: HashRefreshToken(request.CurrentRefreshToken);

			await refreshTokenRepository.RevokeAllForUserAsync(request.UserId, keepTokenHash);

			return Result.Success();
		}

		public Task RevokeAllSessionsAsync(int userId) => refreshTokenRepository.RevokeAllForUserAsync(userId);

		private async Task<UserCredentials> GetOrCreateExternalUserAsync(ExternalIdentity identity, string? displayNameHint)
		{
			var credentials = await userRepository.GetCredentialsByIdentityAsync(identity.Provider, identity.ProviderKey);

			if (credentials is not null)
			{
				await identityRepository.TouchLastLoginAsync(identity.Provider, identity.ProviderKey);
				return credentials;
			}

			var displayName = PickDisplayName(identity.DisplayName, displayNameHint);

			try
			{
				var userId = await userRepository.CreateWithIdentityAsync(displayName, null, identity.Provider, identity.ProviderKey);
				return new UserCredentials(userId, displayName, null, false);
			}
			catch (DuplicateKeyException)
			{
				// A parallel first sign-in with the same identity created the account first.
				return await userRepository.GetCredentialsByIdentityAsync(identity.Provider, identity.ProviderKey)
					?? throw new InvalidOperationException("External identity vanished right after a duplicate-key conflict.");
			}
		}

		private async Task<AuthTokens> IssueTokensAsync(UserModel user, ClientInfo client)
		{
			var (refreshToken, refreshHash) = GenerateRefreshToken();
			var now = DateTime.UtcNow;
			var refreshExpiresAt = now + _options.RefreshTokenLifetime;

			var record = new NewRefreshToken(
				user.Id,
				refreshHash,
				client.ClientType,
				client.DeviceName,
				now,
				refreshExpiresAt
			);

			await refreshTokenRepository.AddAsync(record);

			var accessToken = tokenService.CreateAccessToken(user);
			return new AuthTokens(accessToken.Value, accessToken.ExpiresAt, refreshToken, refreshExpiresAt, user);
		}

		private static (string Token, byte[] Hash) GenerateRefreshToken()
		{
			var token = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));
			return (token, HashRefreshToken(token));
		}

		private static byte[] HashRefreshToken(string token) => SHA256.HashData(Encoding.UTF8.GetBytes(token));

		private static string PickDisplayName(string? fromProvider, string? hint)
		{
			var name = !string.IsNullOrWhiteSpace(fromProvider)
				? fromProvider.Trim()
				: hint?.Trim();

			if (string.IsNullOrEmpty(name)) return DefaultDisplayName;

			return name.Length > AuthValidation.MaxDisplayNameLength
				? name[..AuthValidation.MaxDisplayNameLength]
				: name;
		}

		private static Result ValidateAll(params Result[] results)
		{
			foreach (var result in results)
			{
				if (!result.Successful) return result;
			}

			return Result.Success();
		}
	}
}

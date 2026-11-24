using System.Net;
using System.Net.Http.Json;
using AutoMapper;
using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;
using CAuthTokensResponse = VocabularyTrainer.Api.Contract.Auth.AuthTokensResponse;
using CClientInfoDto = VocabularyTrainer.Api.Contract.Auth.ClientInfoDto;
using CLoginRequest = VocabularyTrainer.Api.Contract.Auth.LoginRequest;
using CRefreshTokenRequest = VocabularyTrainer.Api.Contract.Auth.RefreshTokenRequest;
using CRegisterRequest = VocabularyTrainer.Api.Contract.Auth.RegisterRequest;
using CResetPasswordRequest = VocabularyTrainer.Api.Contract.Auth.ResetPasswordRequest;
using CSendCodeRequest = VocabularyTrainer.Api.Contract.Auth.SendCodeRequest;
using CVerificationPurpose = VocabularyTrainer.Api.Contract.Auth.VerificationPurpose;

namespace VocabularyTrainer.WinApp.ApiClient.Auth
{
	// Owns the session: talks to api/auth over a plain HttpClient (no AuthHeaderHandler, to avoid recursion)
	// and hands access tokens to AuthHeaderHandler for all other calls.
	internal sealed class AccountClient(
		HttpClient authHttpClient,
		ITokenStore tokenStore,
		IMapper mapper,
		CClientInfoDto client) : IAccountClient, IAccessTokenProvider
	{
		// Renew slightly before expiry so a token does not die in flight.
		private static readonly TimeSpan ExpiryMargin = TimeSpan.FromSeconds(30);

		private readonly SemaphoreSlim _refreshLock = new(1, 1);
		private string? _accessToken;
		private DateTime _accessTokenExpiresAt;
		private string? _refreshToken;

		// The token this instance rotated away from. If the store still holds it, our own save failed:
		// it is not a newer token from another instance and must never be presented again.
		private string? _rotatedAwayToken;

		public UserModel? CurrentUser { get; private set; }

		internal string? RefreshToken => _refreshToken;

		public async Task<bool> TryRestoreSessionAsync()
		{
			var storedToken = tokenStore.Load();

			if (string.IsNullOrEmpty(storedToken)) return false;

			_refreshToken = storedToken;

			try
			{
				return await RefreshAccessTokenAsync(null, CancellationToken.None) is not null;
			}
			catch (Exception ex) when (ApiFailures.IsHttpFailure(ex))
			{
				// API unreachable or too slow: keep the stored token for the next launch, fall back to the login form now.
				_refreshToken = null;
				return false;
			}
		}

		public Task<Result> LoginAsync(string login, string password)
		{
			return AuthenticateAsync("api/auth/login", new CLoginRequest(login, password, client));
		}

		public Task<Result> SendRegistrationCodeAsync(string email) => SendCodeAsync(email, CVerificationPurpose.Register);

		public Task<Result> RegisterAsync(string email, string code, string password, string displayName)
		{
			var request = new CRegisterRequest(IdentityProviders.Email, email, code, password, displayName, client);
			return AuthenticateAsync("api/auth/register", request);
		}

		public Task<Result> SendPasswordResetCodeAsync(string email) => SendCodeAsync(email, CVerificationPurpose.ResetPassword);

		public async Task<Result> ResetPasswordAsync(string email, string code, string newPassword)
		{
			var request = new CResetPasswordRequest(IdentityProviders.Email, email, code, newPassword);
			using var response = await authHttpClient.PostAsJsonAsync("api/auth/password/reset", request);

			return await ApiFailures.ToFailureAsync(response) ?? Result.Success();
		}

		public async Task LogoutAsync()
		{
			var refreshToken = _refreshToken;
			ClearSession();

			if (refreshToken is null) return;

			try
			{
				using var response = await authHttpClient.PostAsJsonAsync("api/auth/logout", new CRefreshTokenRequest(refreshToken));
			}
			catch (Exception ex) when (ApiFailures.IsHttpFailure(ex))
			{
				// Best effort: the local session is already gone; the server token simply expires.
			}
		}

		public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
		{
			if (IsAccessTokenFresh()) return _accessToken;

			if (_refreshToken is null) return null;

			return await RefreshAccessTokenAsync(_accessToken, cancellationToken);
		}

		public async Task<string?> RefreshAccessTokenAsync(string? rejectedToken, CancellationToken cancellationToken)
		{
			await _refreshLock.WaitAsync(cancellationToken);

			try
			{
				// Several requests can fail at once; the first one refreshes, the rest reuse its result.
				if (_accessToken is not null && _accessToken != rejectedToken && IsAccessTokenFresh())
				{
					return _accessToken;
				}

				AdoptTokenRotatedElsewhere();

				if (_refreshToken is null) return null;

				var request = new CRefreshTokenRequest(_refreshToken);
				using var response = await authHttpClient.PostAsJsonAsync("api/auth/refresh", request, cancellationToken);

				if (response.StatusCode == HttpStatusCode.Unauthorized)
				{
					ClearSession();
					return null;
				}

				response.EnsureSuccessStatusCode();

				var tokens = await response.Content.ReadFromJsonAsync<CAuthTokensResponse>(cancellationToken);
				ApplyTokens(tokens!);

				return _accessToken;
			}
			finally
			{
				_refreshLock.Release();
			}
		}

		internal void UpdateCurrentUser(UserModel user) => CurrentUser = user;

		// Used after the server already revoked every session: there is nothing left to revoke remotely.
		internal void ForgetSession() => ClearSession();

		// Another app instance sharing the token store may have rotated the refresh token already.
		// Presenting the old one would look like token theft to the server and revoke every session.
		private void AdoptTokenRotatedElsewhere()
		{
			var storedToken = tokenStore.Load();

			if (!string.IsNullOrEmpty(storedToken) && storedToken != _refreshToken && storedToken != _rotatedAwayToken)
			{
				_refreshToken = storedToken;
			}
		}

		private async Task<Result> SendCodeAsync(string email, CVerificationPurpose purpose)
		{
			var request = new CSendCodeRequest(IdentityProviders.Email, email, purpose);
			using var response = await authHttpClient.PostAsJsonAsync("api/auth/codes", request);

			return await ApiFailures.ToFailureAsync(response) ?? Result.Success();
		}

		private async Task<Result> AuthenticateAsync<TRequest>(string url, TRequest request)
		{
			using var response = await authHttpClient.PostAsJsonAsync(url, request);
			var failure = await ApiFailures.ToFailureAsync(response);

			if (failure is not null) return failure;

			var tokens = await response.Content.ReadFromJsonAsync<CAuthTokensResponse>();
			ApplyTokens(tokens!);

			return Result.Success();
		}

		private bool IsAccessTokenFresh()
		{
			return _accessToken is not null && DateTime.UtcNow < _accessTokenExpiresAt - ExpiryMargin;
		}

		private void ApplyTokens(CAuthTokensResponse tokens)
		{
			_accessToken = tokens.AccessToken;
			_accessTokenExpiresAt = tokens.AccessTokenExpiresAt.ToUniversalTime();
			_rotatedAwayToken = _refreshToken;
			_refreshToken = tokens.RefreshToken;
			CurrentUser = mapper.Map<UserModel>(tokens.User);

			tokenStore.Save(tokens.RefreshToken);
		}

		private void ClearSession()
		{
			_accessToken = null;
			_refreshToken = null;
			_rotatedAwayToken = null;
			CurrentUser = null;

			tokenStore.Clear();
		}
	}
}

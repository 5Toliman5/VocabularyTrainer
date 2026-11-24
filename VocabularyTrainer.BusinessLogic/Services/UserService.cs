using Common.Wrappers;
using VocabularyTrainer.BusinessLogic.Services.Auth;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class UserService(
		IUserRepository userRepository,
		IUserIdentityRepository identityRepository,
		IVerificationCodeService verificationCodeService) : IUserService
	{
		public async Task<Result<UserModel>> GetByIdAsync(int userId)
		{
			var user = await userRepository.GetByIdAsync(userId);

			return user is not null
				? Result<UserModel>.Success(user)
				: Result<UserModel>.Failure($"User {userId} was not found.", ResultErrorKind.NotFound);
		}

		public async Task<Result<UserModel>> UpdateDisplayNameAsync(int userId, string displayName)
		{
			var validation = AuthValidation.ValidateDisplayName(displayName);

			if (!validation.Successful)
			{
				return Result<UserModel>.Failure(validation);
			}

			var trimmed = displayName.Trim();
			var updated = await userRepository.UpdateDisplayNameAsync(userId, trimmed);

			return updated > 0
				? Result<UserModel>.Success(new UserModel(userId, trimmed))
				: Result<UserModel>.Failure($"User {userId} was not found.", ResultErrorKind.NotFound);
		}

		public Task<List<UserIdentityDto>> GetIdentitiesAsync(int userId) => identityRepository.GetByUserAsync(userId);

		public async Task<Result> SendLinkCodeAsync(int userId, string channel, string target)
		{
			if (!verificationCodeService.IsChannelSupported(channel))
			{
				return Result.Failure($"Codes cannot be sent via '{channel}'.", ResultErrorKind.Validation);
			}

			if (!LoginNormalizer.TryNormalize(channel, target, out var normalized))
			{
				return AuthValidation.InvalidTarget(channel);
			}

			if (await identityRepository.ExistsAsync(channel, normalized))
			{
				return Result.Failure($"This {channel} is already linked to an account.", ResultErrorKind.Conflict);
			}

			return await verificationCodeService.SendAsync(channel, normalized, VerificationPurpose.LinkIdentity);
		}

		public async Task<Result<UserIdentityDto>> LinkIdentityAsync(LinkIdentityRequest request)
		{
			if (!LoginNormalizer.TryNormalize(request.Channel, request.Target, out var normalized))
			{
				return Result<UserIdentityDto>.Failure(AuthValidation.InvalidTarget(request.Channel));
			}

			var codeCheck = await verificationCodeService.VerifyAsync(
				request.Channel,
				normalized,
				VerificationPurpose.LinkIdentity,
				request.Code
			);

			if (!codeCheck.Successful)
			{
				return Result<UserIdentityDto>.Failure(codeCheck);
			}

			int identityId;

			try
			{
				identityId = await identityRepository.AddAsync(request.UserId, request.Channel, normalized);
			}
			catch (DuplicateKeyException)
			{
				return Result<UserIdentityDto>.Failure($"This {request.Channel} is already linked to an account.", ResultErrorKind.Conflict);
			}

			var identity = new UserIdentityDto(identityId, request.Channel, normalized, DateTime.UtcNow, null);
			return Result<UserIdentityDto>.Success(identity);
		}

		public async Task<Result> UnlinkIdentityAsync(int userId, int identityId)
		{
			var identities = await identityRepository.GetByUserAsync(userId);

			if (identities.All(i => i.Id != identityId))
			{
				return Result.Failure("Sign-in method was not found.", ResultErrorKind.NotFound);
			}

			var deleted = await identityRepository.DeleteUnlessLastAsync(identityId, userId);

			return deleted > 0
				? Result.Success()
				: Result.Failure("The last sign-in method cannot be removed.", ResultErrorKind.Validation);
		}
	}
}

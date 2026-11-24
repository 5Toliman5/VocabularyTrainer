using Common.Wrappers;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
	public class ApiUserService(IUserService userService) : IApiUserService
	{
		public Task<Result<UserModel>> GetByIdAsync(int userId) => userService.GetByIdAsync(userId);

		public Task<Result<UserModel>> UpdateDisplayNameAsync(int userId, string displayName) =>
			userService.UpdateDisplayNameAsync(userId, displayName);

		public Task<List<UserIdentityDto>> GetIdentitiesAsync(int userId) => userService.GetIdentitiesAsync(userId);

		public Task<Result> SendLinkCodeAsync(int userId, string channel, string target) =>
			userService.SendLinkCodeAsync(userId, channel, target);

		public Task<Result<UserIdentityDto>> LinkIdentityAsync(LinkIdentityRequest request) => userService.LinkIdentityAsync(request);

		public Task<Result> UnlinkIdentityAsync(int userId, int identityId) => userService.UnlinkIdentityAsync(userId, identityId);
	}
}

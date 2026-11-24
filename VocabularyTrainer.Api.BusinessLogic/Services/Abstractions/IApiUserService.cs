using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Api.BusinessLogic.Services.Abstractions
{
	public interface IApiUserService
	{
		Task<Result<UserModel>> GetByIdAsync(int userId);
		Task<Result<UserModel>> UpdateDisplayNameAsync(int userId, string displayName);
		Task<List<UserIdentityDto>> GetIdentitiesAsync(int userId);
		Task<Result> SendLinkCodeAsync(int userId, string channel, string target);
		Task<Result<UserIdentityDto>> LinkIdentityAsync(LinkIdentityRequest request);
		Task<Result> UnlinkIdentityAsync(int userId, int identityId);
	}
}

using Common.Web.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public class ApiUserService(IUserService userService) : IApiUserService
    {
        public async Task<ApiOperationResult<UserModel>> GetAsync(string userName)
        {
            var result = await userService.GetAsync(userName);
            if (!result.Successful)
                return ApiOperationResult<UserModel>.Failure(result.ErrorMessage!, ApiErrorType.NotFound);

            return ApiOperationResult<UserModel>.Success(result.Value);
        }
    }
}

using Common.Wrappers;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public class ApiUserService(IUserService userService) : IApiUserService
    {
        public Task<Result<UserModel>> GetAsync(string userName)
            => userService.GetAsync(userName);
    }
}

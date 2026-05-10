using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Api.BusinessLogic.Services.Abstractions
{
    public interface IApiUserService
    {
        Task<Result<UserModel>> GetAsync(string userName);
    }
}

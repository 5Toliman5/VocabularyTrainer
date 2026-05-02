using Common.Web.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public interface IApiUserService
    {
        Task<ApiOperationResult<UserModel>> GetAsync(string userName);
    }
}

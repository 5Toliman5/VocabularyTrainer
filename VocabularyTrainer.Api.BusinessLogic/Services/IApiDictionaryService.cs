using Common.Web.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public interface IApiDictionaryService
    {
        Task<List<DictionaryDto>> GetAllAsync(int userId);
        Task<ApiOperationResult<DictionaryDto>> AddAsync(AddDictionaryRequest request);
        Task<ApiOperationResult> UpdateAsync(UpdateDictionaryRequest request);
        Task DeleteAsync(int dictionaryId, int userId);
    }
}

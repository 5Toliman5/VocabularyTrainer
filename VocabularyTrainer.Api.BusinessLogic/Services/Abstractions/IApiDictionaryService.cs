using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Api.BusinessLogic.Services.Abstractions
{
    public interface IApiDictionaryService
    {
        Task<List<DictionaryDto>> GetAllAsync(int userId);
        Task<DictionaryDto?> GetByIdAsync(int dictionaryId, int userId);
        Task<Result<DictionaryDto>> AddAsync(AddDictionaryRequest request);
        Task<Result> UpdateAsync(UpdateDictionaryRequest request);
        Task DeleteAsync(int dictionaryId, int userId);
    }
}

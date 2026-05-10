using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Api.BusinessLogic.Services.Abstractions
{
    public interface IApiWordService
    {
        Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null);
        Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request);
        Task<Result<int>> AddAsync(AddWordRequest request);
        Task<Result> DeleteAsync(int wordId, int userId);
    }
}

using Common.Wrappers;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    // Thin facade over IWordService.
    public class ApiWordService(IWordService wordService) : IApiWordService
    {
        public Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
            => wordService.GetAllAsync(userId, dictionaryId);

        public Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request)
            => wordService.GetPagedAsync(request);

        public Task<Result<int>> AddAsync(AddWordRequest request)
            => wordService.AddAsync(request);

        public Task<Result> DeleteAsync(int wordId, int userId)
            => wordService.DeleteAsync(wordId, userId);
    }
}

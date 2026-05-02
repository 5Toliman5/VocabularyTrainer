using Common.Web.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public class ApiWordService(IWordService wordService) : IApiWordService
    {
        public Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
            => wordService.GetAllAsync(userId, dictionaryId);

        public Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request)
            => wordService.GetPagedAsync(request);

        public Task AddAsync(AddWordRequest request)
            => wordService.AddAsync(request);

        public Task DeleteAsync(UserWordKey key)
            => wordService.DeleteAsync(key);

        public Task UpdateWeightAsync(UpdateWordWeightRequest request)
            => wordService.UpdateWeightAsync(request);
    }
}

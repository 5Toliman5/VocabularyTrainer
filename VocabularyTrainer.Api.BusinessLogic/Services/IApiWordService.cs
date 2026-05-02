using Common.Web.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public interface IApiWordService
    {
        Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null);
        Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request);
        Task AddAsync(AddWordRequest request);
        Task DeleteAsync(UserWordKey key);
        Task UpdateWeightAsync(UpdateWordWeightRequest request);
    }
}

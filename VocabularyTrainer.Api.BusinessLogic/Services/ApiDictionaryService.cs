using Common.Wrappers;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public class ApiDictionaryService(IDictionaryService dictionaryService) : IApiDictionaryService
    {
        public Task<List<DictionaryDto>> GetAllAsync(int userId)
            => dictionaryService.GetAllAsync(userId);

        public Task<DictionaryDto?> GetByIdAsync(int dictionaryId, int userId)
            => dictionaryService.GetByIdAsync(dictionaryId, userId);

        public Task<Result<DictionaryDto>> AddAsync(AddDictionaryRequest request)
            => dictionaryService.AddAsync(request);

        public Task<Result> UpdateAsync(UpdateDictionaryRequest request)
            => dictionaryService.UpdateAsync(request);

        public Task DeleteAsync(int dictionaryId, int userId)
            => dictionaryService.DeleteAsync(dictionaryId, userId);
    }
}

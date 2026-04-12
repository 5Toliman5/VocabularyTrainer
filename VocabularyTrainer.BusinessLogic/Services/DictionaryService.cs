using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	/// <summary>Implements dictionary management business logic.</summary>
	public class DictionaryService(IDictionaryRepository repository) : IDictionaryService
	{
        /// <inheritdoc/>
        public Task<List<DictionaryDto>> GetAllAsync(int userId)
        {
            return repository.GetAllAsync(userId);
        }

        /// <inheritdoc/>
        public async Task<DictionaryDto> AddAsync(AddDictionaryRequest request)
		{
			var id = await repository.AddAsync(request);
			return new DictionaryDto(id, request.Name, request.LanguageCode);
		}

        /// <inheritdoc/>
        public Task UpdateAsync(UpdateDictionaryRequest request)
        {
            return repository.UpdateAsync(request);
        }

        /// <inheritdoc/>
        public Task DeleteAsync(int dictionaryId, int userId)
        {
            return repository.DeleteAsync(dictionaryId, userId);
        }
    }
}

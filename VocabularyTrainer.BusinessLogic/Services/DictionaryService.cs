using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class DictionaryService(IDictionaryRepository repository) : IDictionaryService
	{
		public Task<List<DictionaryDto>> GetAllAsync(int userId)
		{
			return repository.GetAllAsync(userId);
		}

		public async Task<DictionaryDto> AddAsync(AddDictionaryRequest request)
		{
			var id = await repository.AddAsync(request);
			return new DictionaryDto(id, request.Name, request.LanguageCode);
		}

		public Task UpdateAsync(UpdateDictionaryRequest request)
		{
			return repository.UpdateAsync(request);
		}

		public Task DeleteAsync(int dictionaryId, int userId)
		{
			return repository.DeleteAsync(dictionaryId, userId);
		}
	}
}

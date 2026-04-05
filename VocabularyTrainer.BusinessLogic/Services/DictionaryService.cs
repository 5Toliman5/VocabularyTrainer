using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class DictionaryService(IDictionaryRepository repository) : IDictionaryService
	{
		public Task<List<DictionaryDto>> GetAllAsync(int userId) =>
			repository.GetAllAsync(userId);

		public async Task<DictionaryDto> AddAsync(int userId, string name, string? languageCode)
		{
			var request = new AddDictionaryRequest(userId, name, languageCode);
			var id = await repository.AddAsync(request);
			return new DictionaryDto(id, name, languageCode);
		}

		public Task UpdateAsync(int dictionaryId, string name, string? languageCode) =>
			repository.UpdateAsync(new UpdateDictionaryRequest(dictionaryId, name, languageCode));

		public Task DeleteAsync(int dictionaryId) =>
			repository.DeleteAsync(dictionaryId);
	}
}

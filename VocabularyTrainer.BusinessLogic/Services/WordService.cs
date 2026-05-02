using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class WordService(IWordRepository repository) : IWordService
	{
		public Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
			=> repository.GetAllAsync(userId, dictionaryId);

		public Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request)
			=> repository.GetPagedAsync(request);

		public Task AddAsync(AddWordRequest request)
			=> repository.AddAsync(request);

		public Task DeleteAsync(UserWordKey key)
			=> repository.DeleteAsync(key);

		public Task UpdateWeightAsync(UpdateWordWeightRequest request)
			=> repository.UpdateWeightAsync(request);
	}
}

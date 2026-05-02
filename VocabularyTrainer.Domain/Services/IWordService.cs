using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IWordService
	{
		Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null);
		Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request);
		Task AddAsync(AddWordRequest request);
		Task DeleteAsync(UserWordKey key);
		Task UpdateWeightAsync(UpdateWordWeightRequest request);
	}
}

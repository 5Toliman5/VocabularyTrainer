using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IWordRepository
	{
		Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null);
		Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request);
		Task AddAsync(AddWordRequest request);
		Task DeleteAsync(UserWordKey request);
		Task UpdateWeightAsync(UpdateWordWeightRequest request);
	}
}

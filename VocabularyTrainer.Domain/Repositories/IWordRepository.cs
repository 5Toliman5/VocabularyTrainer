using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IWordRepository
	{
		Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null);

		Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request);

		Task<List<WordDto>> GetByIdsAsync(int userId, IReadOnlyCollection<int> wordIds);

		Task<int> AddAsync(AddWordRequest request, string normalizedText, string languageCode);

		Task<int> DeleteAsync(int wordId, int userId);
	}
}

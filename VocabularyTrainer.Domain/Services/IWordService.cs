using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IWordService
	{
		Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null);
		Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request);
		Task<Result<int>> AddAsync(AddWordRequest request);
		Task<Result> DeleteAsync(int wordId, int userId);
	}
}

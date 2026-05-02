using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IDictionaryRepository
	{
		Task<List<DictionaryDto>> GetAllAsync(int userId);
		Task<Result<int>> AddAsync(AddDictionaryRequest request);
		Task<Result> UpdateAsync(UpdateDictionaryRequest request);
		Task DeleteAsync(int dictionaryId, int userId);
	}
}

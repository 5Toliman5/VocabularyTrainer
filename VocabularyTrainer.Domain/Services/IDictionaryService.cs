using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IDictionaryService
	{
		Task<List<DictionaryDto>> GetAllAsync(int userId);
		Task<Result<DictionaryDto>> AddAsync(AddDictionaryRequest request);
		Task<Result> UpdateAsync(UpdateDictionaryRequest request);
		Task DeleteAsync(int dictionaryId, int userId);
	}
}

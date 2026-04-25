using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IDictionaryService
	{
		Task<List<DictionaryDto>> GetAllAsync(int userId);
		Task<DictionaryDto> AddAsync(AddDictionaryRequest request);
		Task UpdateAsync(UpdateDictionaryRequest request);
		Task DeleteAsync(int dictionaryId, int userId);
	}
}

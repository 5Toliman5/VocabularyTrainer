using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IDictionaryService
	{
		Task<List<DictionaryDto>> GetAllAsync(int userId);

		Task<DictionaryDto> AddAsync(int userId, string name, string? languageCode);

		Task UpdateAsync(int dictionaryId, string name, string? languageCode);

		Task DeleteAsync(int dictionaryId);
	}
}

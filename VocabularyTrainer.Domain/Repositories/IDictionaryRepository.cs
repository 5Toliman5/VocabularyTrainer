using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IDictionaryRepository
	{
		Task<List<DictionaryDto>> GetAllAsync(int userId);

		Task<DictionaryDto?> GetByIdAsync(int dictionaryId, int userId);

		Task<int> AddAsync(AddDictionaryRequest request, int algorithmPersistenceId);

		Task<int> UpdateAsync(UpdateDictionaryRequest request, int algorithmPersistenceId);

		Task DeleteAsync(int dictionaryId, int userId);
	}
}

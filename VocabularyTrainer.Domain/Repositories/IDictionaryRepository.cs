using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IDictionaryRepository
	{
		Task<List<DictionaryDto>> GetAllAsync(int userId);

		Task<int> AddAsync(AddDictionaryRequest request);

		Task UpdateAsync(UpdateDictionaryRequest request);

		Task DeleteAsync(int dictionaryId);
	}
}

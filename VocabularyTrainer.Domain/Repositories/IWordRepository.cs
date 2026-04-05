using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IWordRepository
	{
		Task<List<WordDto>> GetAllAsync(int userId);

		Task AddAsync(AddWordRequest request);

		Task DeleteAsync(EditWordRequest request);

		Task UpdateWeightAsync(UpdateWordWeightRequest request);
	}
}

using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IAlgorithmCatalog
	{
		Task<IReadOnlyList<AlgorithmInfo>> GetAllAsync();
	}
}

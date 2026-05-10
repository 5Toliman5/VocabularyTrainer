using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IWordWeightBasedParamsRepository
	{
		Task<List<(int WordId, int? Weight)>> GetCandidatePoolAsync(int userId, int? dictionaryId);
		Task<WordWeightBasedParams?> GetAsync(int wordId);
		Task UpsertAsync(int wordId, int weight);
	}
}

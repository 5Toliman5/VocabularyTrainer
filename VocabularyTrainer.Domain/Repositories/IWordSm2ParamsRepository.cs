using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IWordSm2ParamsRepository
	{
		Task<List<int>> GetCandidateIdsAsync(int userId, int? dictionaryId, int limit);
		Task<WordSm2Params?> GetAsync(int wordId);
		Task UpsertAsync(WordSm2Params @params);
	}
}

using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface ITrainingClient
	{
		Task<List<WordDto>> GetCandidatesAsync(int userId, DictionaryScope scope, int limit);
		Task ApplyReviewAsync(ReviewWordRequest request);
		Task<IReadOnlyList<ReviewGrade>> GetSupportedGradesAsync(int dictionaryId, int userId);
	}
}

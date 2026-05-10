using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	// Strategy interface: each training algorithm implements its own scheduling
	// and review-handling logic with its own satellite parameter table.
	public interface IWordTrainingAlgorithm
	{
		AlgorithmInfo Info { get; }

		// Loads up to `limit` next-best cards for this user/dictionary in
		// algorithm-specific order. Returned cards are ready to be shown to the user.
		Task<List<WordDto>> GetTrainingCandidatesAsync(int userId, int? dictionaryId, int limit);

		// Applies the user's grade to the card and persists updated parameters.
		Task ApplyReviewAsync(WordDto card, ReviewGrade grade);
	}
}

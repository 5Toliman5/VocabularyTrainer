using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	/// <summary>Contract for ordering a word list in a weight-aware shuffled sequence.</summary>
	public interface IWordsShuffleService
	{
		/// <summary>Returns the word list reordered so that lower-weight words appear earlier on average.</summary>
		List<WordDto> Shuffle(List<WordDto> words);
	}
}

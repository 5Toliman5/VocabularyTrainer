using Common.Extensions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	/// <summary>Implements weight-aware word shuffling.</summary>
	public class WordsShuffleService : IWordsShuffleService
	{
		/// <inheritdoc/>
		public List<WordDto> Shuffle(List<WordDto> words)
		{
			if (words.IsNullOrEmpty())
			{
				return [];
			}

			var rng = new Random();

			return words
				.OrderBy(word => rng.NextDouble() / (word.Weight + 1))
				.ToList();
		}
	}
}

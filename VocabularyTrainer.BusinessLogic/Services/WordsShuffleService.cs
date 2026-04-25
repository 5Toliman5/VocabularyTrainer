using Common.Extensions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class WordsShuffleService : IWordsShuffleService
	{
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

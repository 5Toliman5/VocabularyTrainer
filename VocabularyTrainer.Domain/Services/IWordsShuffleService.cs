using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IWordsShuffleService
	{
		List<WordDto> Shuffle(List<WordDto> words);
	}
}

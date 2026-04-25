using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IWordTrainerService
	{
		int GetWordsCount();
		Task AddWordAsync(WordDto word, int dictionaryId);
		Task DeleteCurrentWordAsync();
		WordDto? GetCurrentWord();
		Task<WordDto?> GetNewWordAsync();
		Task LoadWordsAsync();
		void SetUser(UserModel user);
		void SetDictionary(int? dictionaryId);
		Task UpdateCurrentWordAsync(UpdateWeightType updateWeightType);
	}
}

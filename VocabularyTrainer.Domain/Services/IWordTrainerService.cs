using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	/// <summary>Business logic contract for the word training session.</summary>
	public interface IWordTrainerService
	{
		/// <summary>Returns the number of words remaining in the current training queue.</summary>
		int GetWordsCount();

		/// <summary>Adds a word to the specified dictionary, replacing any existing entry with the same value in that dictionary.</summary>
		Task AddWordAsync(WordDto word, int dictionaryId);

		/// <summary>Deletes the current word from the user's dictionary.</summary>
		Task DeleteCurrentWordAsync();

		/// <summary>Returns a value-only projection of the current word, or <see langword="null"/> if no word is active.</summary>
		WordDto? GetCurrentWord();

		/// <summary>Removes the current word from the queue and returns the next word, reloading from the database when the queue is exhausted.</summary>
		Task<WordDto?> GetNewWordAsync();

		/// <summary>Loads all words for the active session from the database, shuffled by weight.</summary>
		Task LoadWordsAsync();

		/// <summary>Sets the active user and starts a new training session, discarding any previous state.</summary>
		void SetUser(UserModel user);

		/// <summary>Filters training to the specified dictionary, resetting the word queue when the selection changes.</summary>
		void SetDictionary(int? dictionaryId);

		/// <summary>Updates the training weight of the current word in the specified direction.</summary>
		Task UpdateCurrentWordAsync(UpdateWeightType updateWeightType);
	}
}

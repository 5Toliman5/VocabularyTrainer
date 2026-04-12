using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	/// <summary>Persistence contract for word operations.</summary>
	public interface IWordRepository
	{
		/// <summary>Returns all words for a user, optionally filtered to a single dictionary.</summary>
		Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null);

		/// <summary>Inserts a word and its user-dictionary association within a single transaction.</summary>
		Task AddAsync(AddWordRequest request);

		/// <summary>Removes a user-word association and deletes the underlying word if no other users reference it.</summary>
		Task DeleteAsync(UserWordKey request);

		/// <summary>Increments or decrements the training weight of a word for a specific user and dictionary.</summary>
		Task UpdateWeightAsync(UpdateWordWeightRequest request);
	}
}

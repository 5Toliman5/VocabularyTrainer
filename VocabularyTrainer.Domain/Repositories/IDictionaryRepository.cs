using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	/// <summary>Persistence contract for dictionary operations.</summary>
	public interface IDictionaryRepository
	{
		/// <summary>Returns all dictionaries belonging to the specified user, ordered by name.</summary>
		Task<List<DictionaryDto>> GetAllAsync(int userId);

		/// <summary>Inserts a new dictionary and returns its generated identifier.</summary>
		Task<int> AddAsync(AddDictionaryRequest request);

		/// <summary>Updates the name and language code of an existing dictionary, scoped to its owning user.</summary>
		Task UpdateAsync(UpdateDictionaryRequest request);

		/// <summary>Deletes a dictionary by identifier, restricted to the owning user.</summary>
		Task DeleteAsync(int dictionaryId, int userId);
	}
}

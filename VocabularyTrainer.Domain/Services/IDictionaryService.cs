using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	/// <summary>Business logic contract for dictionary management.</summary>
	public interface IDictionaryService
	{
		/// <summary>Returns all dictionaries for the specified user, ordered by name.</summary>
		Task<List<DictionaryDto>> GetAllAsync(int userId);

		/// <summary>Creates a new dictionary and returns a projection of the created record.</summary>
		Task<DictionaryDto> AddAsync(AddDictionaryRequest request);

		/// <summary>Updates the name and language code of an existing dictionary.</summary>
		Task UpdateAsync(UpdateDictionaryRequest request);

		/// <summary>Deletes a dictionary, restricted to the owning user.</summary>
		Task DeleteAsync(int dictionaryId, int userId);
	}
}

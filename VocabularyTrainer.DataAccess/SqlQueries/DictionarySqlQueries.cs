using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.DataAccess.SqlQueries
{
	/// <summary>SQL query strings for dictionary operations.</summary>
	public static class DictionarySqlQueries
	{
		/// <summary>Selects all dictionaries for a user with their word counts, ordered by name.</summary>
		public const string SelectAllByUser =
			"SELECT d.ID, d.Name, d.LanguageCode, COUNT(uw.ID) AS WordCount " +
			"FROM Dictionaries d " +
			"LEFT JOIN UserWords uw ON uw.DictionaryId = d.ID " +
			"WHERE d.UserId = @UserId " +
			"GROUP BY d.ID, d.Name, d.LanguageCode " +
			"ORDER BY d.Name";

		/// <summary>Inserts a new dictionary and returns its generated ID.</summary>
		public const string Insert =
			$"INSERT INTO Dictionaries(UserId, Name, LanguageCode) OUTPUT INSERTED.ID " +
			$"VALUES(@{nameof(AddDictionaryRequest.UserId)}, @{nameof(AddDictionaryRequest.Name)}, @{nameof(AddDictionaryRequest.LanguageCode)})";

		/// <summary>Updates the name and language code of a dictionary, scoped to its owning user.</summary>
		public const string Update =
			$"UPDATE Dictionaries SET Name = @{nameof(UpdateDictionaryRequest.Name)}, " +
			$"LanguageCode = @{nameof(UpdateDictionaryRequest.LanguageCode)} " +
			$"WHERE ID = @{nameof(UpdateDictionaryRequest.DictionaryId)} AND UserId = @{nameof(UpdateDictionaryRequest.UserId)}";

		/// <summary>Deletes a dictionary by ID, restricted to the owning user.</summary>
		public const string Delete = "DELETE FROM Dictionaries WHERE ID = @DictionaryId AND UserId = @UserId";
	}
}

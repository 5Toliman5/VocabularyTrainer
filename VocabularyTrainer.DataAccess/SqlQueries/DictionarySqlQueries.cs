using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.DataAccess.SqlQueries
{
	public static class DictionarySqlQueries
	{
		public const string SelectAllByUser =
			"SELECT d.ID, d.Name, d.LanguageCode, COUNT(uw.ID) AS WordCount " +
			"FROM Dictionaries d " +
			"LEFT JOIN UserWords uw ON uw.DictionaryId = d.ID " +
			"WHERE d.UserId = @UserId " +
			"GROUP BY d.ID, d.Name, d.LanguageCode " +
			"ORDER BY d.Name";

		public const string Insert =
			$"INSERT INTO Dictionaries(UserId, Name, LanguageCode) OUTPUT INSERTED.ID " +
			$"VALUES(@{nameof(AddDictionaryRequest.UserId)}, @{nameof(AddDictionaryRequest.Name)}, @{nameof(AddDictionaryRequest.LanguageCode)})";

		public const string Update =
			$"UPDATE Dictionaries SET Name = @{nameof(UpdateDictionaryRequest.Name)}, " +
			$"LanguageCode = @{nameof(UpdateDictionaryRequest.LanguageCode)} " +
			$"WHERE ID = @{nameof(UpdateDictionaryRequest.DictionaryId)}";

		public const string Delete = "DELETE FROM Dictionaries WHERE ID = @DictionaryId";
	}
}

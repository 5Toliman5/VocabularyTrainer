using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.DataAccess.SqlQueries
{
	/// <summary>SQL query strings for word and user-word operations.</summary>
	public static class WordSqlQueries
	{
		#region Words

		/// <summary>Inserts a word and returns its generated ID.</summary>
		public const string InsertWord =
			$"INSERT INTO Words(Value, Translation) OUTPUT INSERTED.ID VALUES(@{nameof(AddWordRequest.Value)},@{nameof(AddWordRequest.Translation)})";

		/// <summary>Deletes a word by ID.</summary>
		public const string DeleteWord =
			$"DELETE FROM Words WHERE Id = @{nameof(UserWordKey.WordId)}";

		/// <summary>Selects all words for a user across all their dictionaries.</summary>
		public const string SelectAllWords =
			"SELECT w.Id, w.Value, w.Translation, uw.Weight, uw.DictionaryId, d.Name AS DictionaryName " +
			"FROM Words w " +
			"JOIN UserWords uw ON w.ID = uw.WordId " +
			"JOIN Dictionaries d ON d.ID = uw.DictionaryId " +
			"WHERE uw.UserId = @UserId";

		/// <summary>Selects all words for a user in a specific dictionary.</summary>
		public const string SelectAllWordsByDictionary =
			SelectAllWords + " AND uw.DictionaryId = @DictionaryId";

		#endregion

		#region UserWords

		/// <summary>Increments the training weight of a user-word entry.</summary>
		public const string UpdateWordWeightIncrease =
			"UPDATE UserWords SET Weight = Weight + 1" + UserWordsDictionaryWhereClause;

		/// <summary>Decrements the training weight of a user-word entry.</summary>
		public const string UpdateWordWeightDecrease =
			"UPDATE UserWords SET Weight = Weight - 1" + UserWordsDictionaryWhereClause;

		/// <summary>Deletes a user-word association for a specific dictionary.</summary>
		public const string DeleteUserWord =
			"DELETE FROM UserWords" + UserWordsDictionaryWhereClause;

		/// <summary>Counts how many users have a given word in any of their dictionaries.</summary>
		public const string SelectUserCountOfWord =
			$"SELECT COUNT(*) FROM UserWords" + UserWordsWhereClause;

		/// <summary>Inserts a user-word association with an initial weight of zero.</summary>
		public const string InsertUserWord =
			$"INSERT INTO UserWords(WordId, UserId, DictionaryId, Weight) " +
			$"VALUES(@{nameof(UserWordKey.WordId)}, @{nameof(UserWordKey.UserId)}, @{nameof(UserWordKey.DictionaryId)}, 0)";

		/// <summary>WHERE clause filtering by WordId and UserId.</summary>
		private const string UserWordsWhereClause =
			$" WHERE WordId = @{nameof(UserWordKey.WordId)} AND UserId = @{nameof(UserWordKey.UserId)}";

		/// <summary>WHERE clause filtering by WordId, UserId, and DictionaryId.</summary>
		private const string UserWordsDictionaryWhereClause =
			UserWordsWhereClause + $" AND DictionaryId = @{nameof(UserWordKey.DictionaryId)}";

		#endregion
	}
}

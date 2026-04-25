using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.DataAccess.SqlQueries
{
	public static class WordSqlQueries
	{
		#region Words

		public const string InsertWord =
			$"INSERT INTO Words(Value, Translation) OUTPUT INSERTED.ID VALUES(@{nameof(AddWordRequest.Value)},@{nameof(AddWordRequest.Translation)})";

		public const string DeleteWord =
			$"DELETE FROM Words WHERE Id = @{nameof(UserWordKey.WordId)}";

		public const string SelectAllWords =
			"SELECT w.Id, w.Value, w.Translation, uw.Weight, uw.DictionaryId, d.Name AS DictionaryName, " +
			"d.LanguageCode, uw.DateAdded, uw.DateModified " +
			"FROM Words w " +
			"JOIN UserWords uw ON w.ID = uw.WordId " +
			"JOIN Dictionaries d ON d.ID = uw.DictionaryId " +
			"WHERE uw.UserId = @UserId";

		public const string SelectAllWordsByDictionary =
			SelectAllWords + " AND uw.DictionaryId = @DictionaryId";

		// keyed by enum value — prevents SQL injection from user-supplied sort column names
		public static readonly IReadOnlyDictionary<WordSortBy, string> SortColumns =
			new Dictionary<WordSortBy, string>
			{
				[WordSortBy.Value]          = "w.Value",
				[WordSortBy.Translation]    = "w.Translation",
				[WordSortBy.DictionaryName] = "d.Name",
				[WordSortBy.Language]       = "d.LanguageCode",
				[WordSortBy.DateAdded]      = "uw.DateAdded",
				[WordSortBy.Weight]         = "uw.Weight",
			};

		#endregion

		#region UserWords

		public const string UpdateWordWeightIncrease =
			"UPDATE UserWords SET Weight = Weight + 1" + UserWordsDictionaryWhereClause;

		public const string UpdateWordWeightDecrease =
			"UPDATE UserWords SET Weight = Weight - 1" + UserWordsDictionaryWhereClause;

		public const string DeleteUserWord =
			"DELETE FROM UserWords" + UserWordsDictionaryWhereClause;

		public const string SelectUserCountOfWord =
			$"SELECT COUNT(*) FROM UserWords" + UserWordsWhereClause;

		public const string InsertUserWord =
			$"INSERT INTO UserWords(WordId, UserId, DictionaryId, Weight) " +
			$"VALUES(@{nameof(UserWordKey.WordId)}, @{nameof(UserWordKey.UserId)}, @{nameof(UserWordKey.DictionaryId)}, 0)";

		private const string UserWordsWhereClause =
			$" WHERE WordId = @{nameof(UserWordKey.WordId)} AND UserId = @{nameof(UserWordKey.UserId)}";

		private const string UserWordsDictionaryWhereClause =
			UserWordsWhereClause + $" AND DictionaryId = @{nameof(UserWordKey.DictionaryId)}";

		#endregion
	}
}

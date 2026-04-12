namespace VocabularyTrainer.DataAccess.SqlQueries
{
	/// <summary>SQL query strings for user operations.</summary>
	public static class UserSqlQueries
	{
		/// <summary>Selects a user's ID by their display name.</summary>
		public const string GetUser = "SELECT Id FROM Users WHERE Name = @UserName";
	}
}

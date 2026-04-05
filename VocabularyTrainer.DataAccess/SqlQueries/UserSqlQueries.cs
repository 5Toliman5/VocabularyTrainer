namespace VocabularyTrainer.DataAccess.SqlQueries
{
	public static class UserSqlQueries
	{
		public const string GetUser = "SELECT Id FROM Users WHERE Name = @UserName";
	}
}

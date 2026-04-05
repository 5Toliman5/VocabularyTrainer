namespace VocabularyTrainer.DataAccess.SqlQueries
{
	public class UserSqlQueries
	{
		public const string GetUser = "SELECT Id FROM Users WHERE Name = @UserName";
	}
}

using Microsoft.Data.SqlClient;
using Dapper;
using VocabularyTrainer.DataAccess.SqlQueries;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class UserRepository(string connectionString) : IUserRepository
	{
        public async Task<UserModel?> GetUserAsync(string userName)
		{
			await using var connection = new SqlConnection(connectionString);
			return await connection.QuerySingleOrDefaultAsync<UserModel>(UserSqlQueries.GetUser, new { UserName = userName });
		}
	}
}

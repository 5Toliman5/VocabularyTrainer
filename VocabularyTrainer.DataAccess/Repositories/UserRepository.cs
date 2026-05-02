using Dapper;
using Common.Wrappers;
using Microsoft.Data.SqlClient;
using VocabularyTrainer.DataAccess.SqlQueries;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class UserRepository(string connectionString) : IUserRepository
	{
		public async Task<Result<UserModel>> GetUserAsync(string userName)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				var user = await connection.QuerySingleOrDefaultAsync<UserModel>(UserSqlQueries.GetUser, new { UserName = userName });
				return user is not null
					? Result<UserModel>.Success(user)
					: Result<UserModel>.Failure($"User '{userName}' was not found.");
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve user from the database.", ex);
			}
		}
	}
}

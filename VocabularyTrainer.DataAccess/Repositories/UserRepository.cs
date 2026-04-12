using Dapper;
using Microsoft.Data.SqlClient;
using VocabularyTrainer.DataAccess.SqlQueries;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	/// <summary>SQL Server implementation of <see cref="IUserRepository"/> using Dapper.</summary>
	public class UserRepository(string connectionString) : IUserRepository
	{
		/// <inheritdoc/>
		public async Task<UserModel?> GetUserAsync(string userName)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				return await connection.QuerySingleOrDefaultAsync<UserModel>(UserSqlQueries.GetUser, new { UserName = userName });
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve user from the database.", ex);
			}
		}
	}
}

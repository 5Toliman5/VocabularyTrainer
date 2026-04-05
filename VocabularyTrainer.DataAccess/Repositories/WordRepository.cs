using Dapper;
using Microsoft.Data.SqlClient;
using VocabularyTrainer.DataAccess.SqlQueries;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class WordRepository(string connectionString) : IWordRepository
	{
        public async Task<List<WordDto>> GetAllAsync(int userId)
		{
			await using var connection = new SqlConnection(connectionString);
			var words = await connection.QueryAsync<WordDto>(WordSqlQueries.SelectAllWords, new { UserId = userId });
			return words.ToList();
		}

		public async Task AddAsync(AddWordRequest request)
		{
			await using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();
			await using var transaction = await connection.BeginTransactionAsync();
			try
			{
				var insertedWordId = await connection.ExecuteScalarAsync<int>(WordSqlQueries.InsertWord, request.Word, transaction);
				await connection.ExecuteAsync(WordSqlQueries.InsertUserWord, new EditWordRequest(insertedWordId, request.UserId), transaction);
				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task DeleteAsync(EditWordRequest request)
		{
			await using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();
			await using var transaction = await connection.BeginTransactionAsync();
			try
			{
				await connection.ExecuteAsync(WordSqlQueries.DeleteUserWord, request, transaction);

				var userCount = await connection.ExecuteScalarAsync<int>(WordSqlQueries.SelectUserCountOfWord, request, transaction);
				if (userCount == 0)
					await connection.ExecuteAsync(WordSqlQueries.DeleteWord, request, transaction);

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task UpdateWeightAsync(UpdateWordWeightRequest request)
		{
			var query = string.Format(WordSqlQueries.UpdateWordWeight, request.Operator);
			await using var connection = new SqlConnection(connectionString);
			await connection.ExecuteAsync(query, request);
		}
	}
}

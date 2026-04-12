using Dapper;
using Microsoft.Data.SqlClient;
using VocabularyTrainer.DataAccess.SqlQueries;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	/// <summary>SQL Server implementation of <see cref="IWordRepository"/> using Dapper.</summary>
	public class WordRepository(string connectionString) : IWordRepository
	{
		/// <inheritdoc/>
		public async Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);

				IEnumerable<WordDto> words;
				if (dictionaryId.HasValue)
					words = await connection.QueryAsync<WordDto>(WordSqlQueries.SelectAllWordsByDictionary, new { UserId = userId, DictionaryId = dictionaryId.Value });
				else
					words = await connection.QueryAsync<WordDto>(WordSqlQueries.SelectAllWords, new { UserId = userId });

				return words.ToList();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve words from the database.", ex);
			}
		}

		/// <inheritdoc/>
		public async Task AddAsync(AddWordRequest request)
		{
			await using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();
			await using var transaction = await connection.BeginTransactionAsync();
			try
			{
				var insertedWordId = await connection.ExecuteScalarAsync<int>(WordSqlQueries.InsertWord, request, transaction);
				await connection.ExecuteAsync(WordSqlQueries.InsertUserWord, new UserWordKey(insertedWordId, request.UserId, request.DictionaryId), transaction);
				await transaction.CommitAsync();
			}
			catch (SqlException ex)
			{
				await transaction.RollbackAsync();
				throw new DatabaseException("Failed to add word to the database.", ex);
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		/// <inheritdoc/>
		public async Task DeleteAsync(UserWordKey request)
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
			catch (SqlException ex)
			{
				await transaction.RollbackAsync();
				throw new DatabaseException("Failed to delete word from the database.", ex);
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		/// <inheritdoc/>
		public async Task UpdateWeightAsync(UpdateWordWeightRequest request)
		{
			try
			{
				var query = request.Type == UpdateWeightType.Increase
					? WordSqlQueries.UpdateWordWeightIncrease
					: WordSqlQueries.UpdateWordWeightDecrease;
				await using var connection = new SqlConnection(connectionString);
				await connection.ExecuteAsync(query, request);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update word weight in the database.", ex);
			}
		}
	}
}

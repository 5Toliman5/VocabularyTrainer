using Dapper;
using Microsoft.Data.SqlClient;
using VocabularyTrainer.DataAccess.SqlQueries;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class DictionaryRepository(string connectionString) : IDictionaryRepository
	{
		public async Task<List<DictionaryDto>> GetAllAsync(int userId)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				var result = await connection.QueryAsync<DictionaryDto>(
					DictionarySqlQueries.SelectAllByUser, new { UserId = userId });
				return result.ToList();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve dictionaries from the database.", ex);
			}
		}

		public async Task<int> AddAsync(AddDictionaryRequest request)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				return await connection.ExecuteScalarAsync<int>(DictionarySqlQueries.Insert, request);
			}
			catch (SqlException ex) when (ex.Number is 2627 or 2601)
			{
				throw new DuplicateNameException(ex);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to add dictionary to the database.", ex);
			}
		}

		public async Task UpdateAsync(UpdateDictionaryRequest request)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				await connection.ExecuteAsync(DictionarySqlQueries.Update, request);
			}
			catch (SqlException ex) when (ex.Number is 2627 or 2601)
			{
				throw new DuplicateNameException(ex);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update dictionary in the database.", ex);
			}
		}

		public async Task DeleteAsync(int dictionaryId)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				await connection.ExecuteAsync(DictionarySqlQueries.Delete, new { DictionaryId = dictionaryId });
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to delete dictionary from the database.", ex);
			}
		}
	}
}

using Dapper;
using Common.Wrappers;
using Microsoft.Data.SqlClient;
using VocabularyTrainer.DataAccess.SqlQueries;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class DictionaryRepository(string connectionString) : IDictionaryRepository
	{
		private const int SqlUniqueConstraintViolation = 2627;
		private const int SqlUniqueIndexViolation = 2601;

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

		public async Task<Result<int>> AddAsync(AddDictionaryRequest request)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				var id = await connection.ExecuteScalarAsync<int>(DictionarySqlQueries.Insert, request);
				return Result<int>.Success(id);
			}
			catch (SqlException ex) when (ex.Number is SqlUniqueConstraintViolation or SqlUniqueIndexViolation)
			{
				return Result<int>.Failure("A dictionary with this name already exists.");
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to add dictionary to the database.", ex);
			}
		}

		public async Task<Result> UpdateAsync(UpdateDictionaryRequest request)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				await connection.ExecuteAsync(DictionarySqlQueries.Update, request);
				return Result.Success();
			}
			catch (SqlException ex) when (ex.Number is SqlUniqueConstraintViolation or SqlUniqueIndexViolation)
			{
				return Result.Failure("A dictionary with this name already exists.");
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update dictionary in the database.", ex);
			}
		}

		public async Task DeleteAsync(int dictionaryId, int userId)
		{
			try
			{
				await using var connection = new SqlConnection(connectionString);
				await connection.ExecuteAsync(DictionarySqlQueries.Delete, new { DictionaryId = dictionaryId, UserId = userId });
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to delete dictionary from the database.", ex);
			}
		}
	}
}

using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using VocabularyTrainer.DataAccess.SqlQueries;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class WordRepository(string connectionString) : IWordRepository
	{
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

		public async Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request)
		{
			try
			{
				var orderBy = WordSqlQueries.SortColumns[request.SortBy];
				var direction = request.SortDesc ? "DESC" : "ASC";

				var sql = new StringBuilder(
					"SELECT w.Id, w.Value, w.Translation, uw.Weight, uw.DictionaryId, d.Name AS DictionaryName, " +
					"d.LanguageCode, uw.DateAdded, uw.DateModified, COUNT(*) OVER() AS TotalCount " +
					"FROM Words w " +
					"JOIN UserWords uw ON w.ID = uw.WordId " +
					"JOIN Dictionaries d ON d.ID = uw.DictionaryId " +
					"WHERE uw.UserId = @UserId\n");

				if (request.DictionaryId.HasValue)
					sql.AppendLine("AND uw.DictionaryId = @DictionaryId");

				if (request.Language != null)
					sql.AppendLine("AND d.LanguageCode = @Language");

				if (request.DateFrom.HasValue)
					sql.AppendLine("AND uw.DateAdded >= @DateFrom");

				if (request.DateTo.HasValue)
					sql.AppendLine("AND uw.DateAdded <= @DateTo");

				if (!string.IsNullOrWhiteSpace(request.Search))
					sql.AppendLine("AND (w.Value LIKE @Search OR w.Translation LIKE @Search)");

				sql.AppendLine($"ORDER BY {orderBy} {direction}");
				sql.AppendLine("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

				var parameters = new
				{
					request.UserId,
					request.DictionaryId,
					request.Language,
					request.DateFrom,
					request.DateTo,
					Search = request.Search != null ? $"%{request.Search}%" : null,
					Offset = (request.Page - 1) * request.PageSize,
					request.PageSize,
				};

				await using var connection = new SqlConnection(connectionString);
				var rows = (await connection.QueryAsync<PagedWordRow>(sql.ToString(), parameters)).ToList();

				var totalCount = rows.Count > 0 ? rows[0].TotalCount : 0;
				var items = rows.Select(r =>
					new WordDto(r.Id, r.Value, r.Translation, r.Weight, r.DictionaryId, r.DictionaryName,
					            r.LanguageCode, r.DateAdded, r.DateModified))
					.ToList();

				return new PagedResult<WordDto>(items, totalCount, request.Page, request.PageSize);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve paged words from the database.", ex);
			}
		}

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

		private sealed class PagedWordRow
		{
			public int Id { get; set; }
			public string Value { get; set; } = "";
			public string Translation { get; set; } = "";
			public int Weight { get; set; }
			public int DictionaryId { get; set; }
			public string DictionaryName { get; set; } = "";
			public string? LanguageCode { get; set; }
			public DateTime DateAdded { get; set; }
			public DateTime DateModified { get; set; }
			public int TotalCount { get; set; }
		}
	}
}

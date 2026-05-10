using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class WordRepository(IVocabularyTrainerDbContext dbContext) : IWordRepository
	{
		public async Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
		{
			try
			{
				return await BaseQuery(dbContext, userId, dictionaryId)
					.Select(WordProjections.ToDto)
					.ToListAsync();
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
				var query = BaseQuery(dbContext, request.UserId, request.DictionaryId);

				if (!string.IsNullOrWhiteSpace(request.Language))
				{
					query = query.Where(w => w.LanguageCode == request.Language);
				}

				if (request.DateFrom.HasValue)
				{
					query = query.Where(w => w.DateAdded >= request.DateFrom.Value);
				}

				if (request.DateTo.HasValue)
				{
					query = query.Where(w => w.DateAdded <= request.DateTo.Value);
				}

				if (!string.IsNullOrWhiteSpace(request.Search))
				{
					var pattern = $"%{request.Search}%";
					query = query.Where(w =>
						EF.Functions.Like(w.Value, pattern) ||
						w.Translations.Any(t => EF.Functions.Like(t.Text, pattern)));
				}

				query = ApplySort(query, request.SortBy, request.SortDesc);

				var totalCount = await query.CountAsync();

				var items = await query
					.Skip((request.Page - 1) * request.PageSize)
					.Take(request.PageSize)
					.Select(WordProjections.ToDto)
					.ToListAsync();

				return new PagedResult<WordDto>(items, totalCount, request.Page, request.PageSize);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve paged words from the database.", ex);
			}
		}

		public async Task<List<WordDto>> GetByIdsAsync(int userId, IReadOnlyCollection<int> wordIds)
		{
			if (wordIds.Count == 0)
			{
				return [];
			}

			try
			{
				return await dbContext.Words
					.AsNoTracking()
					.Where(w => w.UserId == userId && wordIds.Contains(w.Id))
					.Select(WordProjections.ToDto)
					.ToListAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve words by ids.", ex);
			}
		}

		public async Task<int> AddAsync(AddWordRequest request, string normalizedText, string languageCode)
		{
			try
			{
				var entity = new Word
				{
					UserId = request.UserId,
					DictionaryId = request.DictionaryId,
					Value = request.Value,
					NormalizedText = normalizedText,
					LanguageCode = languageCode,
					Translations = request.Translations
						.Select(t => new WordTranslation { Text = t.Text, Kind = t.Kind })
						.ToList(),
				};

				dbContext.Words.Add(entity);
				await dbContext.SaveChangesAsync();
				return entity.Id;
			}
			catch (DbUpdateException ex) when (SqlServerDbErrors.IsUniqueViolation(ex))
			{
				throw new DuplicateKeyException(
					"A word with this text already exists in this dictionary.", ex);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to add word to the database.", ex);
			}
		}

		public async Task<int> DeleteAsync(int wordId, int userId)
		{
			try
			{
				return await dbContext.Words
					.Where(w => w.Id == wordId && w.UserId == userId)
					.ExecuteDeleteAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to delete word from the database.", ex);
			}
		}

		private static IQueryable<Word> BaseQuery(IVocabularyTrainerDbContext dbContext, int userId, int? dictionaryId)
		{
			IQueryable<Word> q = dbContext.Words.AsNoTracking().Where(w => w.UserId == userId);
			if (dictionaryId.HasValue)
			{
				q = q.Where(w => w.DictionaryId == dictionaryId.Value);
			}

			return q;
		}

		private static IQueryable<Word> ApplySort(IQueryable<Word> query, WordSortBy sort, bool desc) =>
			sort switch
			{
				WordSortBy.Value => desc ? query.OrderByDescending(w => w.Value) : query.OrderBy(w => w.Value),
				WordSortBy.Translation => desc
					? query.OrderByDescending(w => w.Translations.OrderBy(t => t.Id).Select(t => t.Text).FirstOrDefault())
					: query.OrderBy(w => w.Translations.OrderBy(t => t.Id).Select(t => t.Text).FirstOrDefault()),
				WordSortBy.DictionaryName => desc ? query.OrderByDescending(w => w.Dictionary.Name) : query.OrderBy(w => w.Dictionary.Name),
				WordSortBy.Language => desc ? query.OrderByDescending(w => w.LanguageCode) : query.OrderBy(w => w.LanguageCode),
				WordSortBy.DateAdded => desc ? query.OrderByDescending(w => w.DateAdded) : query.OrderBy(w => w.DateAdded),
				_ => query.OrderByDescending(w => w.DateAdded),
			};
	}
}

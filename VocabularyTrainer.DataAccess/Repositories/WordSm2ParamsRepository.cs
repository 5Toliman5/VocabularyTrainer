using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class WordSm2ParamsRepository(
		IVocabularyTrainerDbContext dbContext,
		IServiceScopeFactory serviceScopeFactory)
		: IWordSm2ParamsRepository
	{
		public async Task<List<int>> GetCandidateIdsAsync(int userId, int? dictionaryId, int limit)
		{
			try
			{
				IQueryable<Word> q = dbContext.Words.AsNoTracking().Where(w => w.UserId == userId);
				if (dictionaryId.HasValue)
				{
					q = q.Where(w => w.DictionaryId == dictionaryId.Value);
				}

				var nowUtc = DateTime.UtcNow;

				return await q
					.Where(w => w.Sm2Params == null
					         || w.Sm2Params.NextDueAt == null
					         || w.Sm2Params.NextDueAt <= nowUtc)
					.OrderBy(w => w.Sm2Params != null && w.Sm2Params.NextDueAt != null ? 0 : 1)
					.ThenBy(w => w.Sm2Params != null ? w.Sm2Params.NextDueAt : null)
					.ThenBy(w => w.Id)
					.Take(limit)
					.Select(w => w.Id)
					.ToListAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to load SM-2 candidate ids.", ex);
			}
		}

		public async Task<WordSm2Params?> GetAsync(int wordId)
		{
			try
			{
				return await dbContext.WordSm2Params
					.AsNoTracking()
					.SingleOrDefaultAsync(p => p.WordId == wordId);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to load SM-2 params.", ex);
			}
		}

		public async Task UpsertAsync(WordSm2Params @params)
		{
			try
			{
				var rows = await dbContext.WordSm2Params
					.Where(p => p.WordId == @params.WordId)
					.ExecuteUpdateAsync(s => s
						.SetProperty(p => p.Repetitions, @params.Repetitions)
						.SetProperty(p => p.IntervalDays, @params.IntervalDays)
						.SetProperty(p => p.EaseFactor, @params.EaseFactor)
						.SetProperty(p => p.Lapses, @params.Lapses)
						.SetProperty(p => p.LastReviewedAt, @params.LastReviewedAt)
						.SetProperty(p => p.NextDueAt, @params.NextDueAt));

				if (rows > 0)
				{
					return;
				}

				try
				{
					dbContext.WordSm2Params.Add(new WordSm2Params
					{
						WordId = @params.WordId,
						Repetitions = @params.Repetitions,
						IntervalDays = @params.IntervalDays,
						EaseFactor = @params.EaseFactor,
						Lapses = @params.Lapses,
						LastReviewedAt = @params.LastReviewedAt,
						NextDueAt = @params.NextDueAt,
					});
					await dbContext.SaveChangesAsync();
				}
				catch (DbUpdateException ex) when (SqlServerDbErrors.IsUniqueViolation(ex))
				{
					await using var retryScope = serviceScopeFactory.CreateAsyncScope();
					var retryDbContext = retryScope.ServiceProvider.GetRequiredService<IVocabularyTrainerDbContext>();
					await retryDbContext.WordSm2Params
						.Where(p => p.WordId == @params.WordId)
						.ExecuteUpdateAsync(s => s
							.SetProperty(p => p.Repetitions, @params.Repetitions)
							.SetProperty(p => p.IntervalDays, @params.IntervalDays)
							.SetProperty(p => p.EaseFactor, @params.EaseFactor)
							.SetProperty(p => p.Lapses, @params.Lapses)
							.SetProperty(p => p.LastReviewedAt, @params.LastReviewedAt)
							.SetProperty(p => p.NextDueAt, @params.NextDueAt));
				}
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to upsert SM-2 params.", ex);
			}
		}
	}
}

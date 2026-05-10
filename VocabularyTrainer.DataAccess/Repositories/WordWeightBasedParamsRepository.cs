using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class WordWeightBasedParamsRepository(
		IVocabularyTrainerDbContext dbContext,
		IServiceScopeFactory serviceScopeFactory)
		: IWordWeightBasedParamsRepository
	{
		public async Task<List<(int WordId, int? Weight)>> GetCandidatePoolAsync(int userId, int? dictionaryId)
		{
			try
			{
				IQueryable<Word> q = dbContext.Words.AsNoTracking().Where(w => w.UserId == userId);
				if (dictionaryId.HasValue)
				{
					q = q.Where(w => w.DictionaryId == dictionaryId.Value);
				}

				var rows = await q
					.Select(w => new
					{
						w.Id,
						Weight = (int?)(w.WeightBasedParams != null ? (int?)w.WeightBasedParams.Weight : null),
					})
					.ToListAsync();

				return rows.Select(x => (x.Id, x.Weight)).ToList();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to load weight-based candidate pool.", ex);
			}
		}

		public async Task<WordWeightBasedParams?> GetAsync(int wordId)
		{
			try
			{
				return await dbContext.WordWeightBasedParams
					.AsNoTracking()
					.SingleOrDefaultAsync(p => p.WordId == wordId);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to load weight-based params.", ex);
			}
		}

		public async Task UpsertAsync(int wordId, int weight)
		{
			try
			{
				var rows = await dbContext.WordWeightBasedParams
					.Where(p => p.WordId == wordId)
					.ExecuteUpdateAsync(s => s.SetProperty(p => p.Weight, weight));

				if (rows > 0)
				{
					return;
				}

				try
				{
					dbContext.WordWeightBasedParams.Add(new WordWeightBasedParams { WordId = wordId, Weight = weight });
					await dbContext.SaveChangesAsync();
				}
				catch (DbUpdateException ex) when (SqlServerDbErrors.IsUniqueViolation(ex))
				{
					await using var retryScope = serviceScopeFactory.CreateAsyncScope();
					var retryDbContext = retryScope.ServiceProvider.GetRequiredService<IVocabularyTrainerDbContext>();
					await retryDbContext.WordWeightBasedParams
						.Where(p => p.WordId == wordId)
						.ExecuteUpdateAsync(s => s.SetProperty(p => p.Weight, weight));
				}
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to upsert weight-based params.", ex);
			}
		}
	}
}

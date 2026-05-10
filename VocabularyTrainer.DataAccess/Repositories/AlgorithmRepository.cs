using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class AlgorithmRepository(IVocabularyTrainerDbContext dbContext) : IAlgorithmRepository
	{
		public async Task<int?> GetIdByCodeAsync(string code)
		{
			try
			{
				return await dbContext.Algorithms
					.AsNoTracking()
					.Where(a => a.Code == code)
					.Select(a => (int?)a.Id)
					.SingleOrDefaultAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to look up algorithm by code.", ex);
			}
		}

		public async Task EnsureExistsAsync(string code)
		{
			if ((await GetIdByCodeAsync(code)).HasValue)
			{
				return;
			}

			try
			{
				dbContext.Algorithms.Add(new Algorithm { Code = code });
				await dbContext.SaveChangesAsync();
			}
			catch (DbUpdateException ex) when (SqlServerDbErrors.IsUniqueViolation(ex))
			{
			}
			catch (SqlException ex)
			{
				throw new DatabaseException($"Failed to insert algorithm '{code}'.", ex);
			}
		}
	}
}

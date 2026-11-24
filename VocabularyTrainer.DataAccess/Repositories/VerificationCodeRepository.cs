using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class VerificationCodeRepository(IVocabularyTrainerDbContext dbContext) : IVerificationCodeRepository
	{
		public async Task<int> AddAsync(NewVerificationCode code)
		{
			var entity = new VerificationCode
			{
				Channel = code.Channel,
				Target = code.Target,
				Purpose = code.Purpose,
				CodeHash = code.CodeHash,
				CreatedAt = code.CreatedAt,
				ExpiresAt = code.ExpiresAt,
			};

			try
			{
				dbContext.VerificationCodes.Add(entity);
				await dbContext.SaveChangesAsync();
				return entity.Id;
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to add verification code to the database.", ex);
			}
		}

		public async Task DeleteAsync(int codeId)
		{
			try
			{
				await dbContext.VerificationCodes
					.Where(c => c.Id == codeId)
					.ExecuteDeleteAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to delete verification code from the database.", ex);
			}
		}

		public async Task<int> DeleteExpiredBeforeAsync(DateTime threshold)
		{
			try
			{
				return await dbContext.VerificationCodes
					.Where(c => c.ExpiresAt < threshold)
					.ExecuteDeleteAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to delete expired verification codes from the database.", ex);
			}
		}

		public async Task<VerificationCodeInfo?> GetLatestAsync(string channel, string target, VerificationPurpose purpose)
		{
			try
			{
				return await dbContext.VerificationCodes
					.AsNoTracking()
					.Where(c => c.Channel == channel && c.Target == target && c.Purpose == purpose)
					.OrderByDescending(c => c.CreatedAt)
					.Select(c => new VerificationCodeInfo(c.Id, c.CodeHash, c.CreatedAt, c.ExpiresAt, c.AttemptCount, c.ConsumedAt))
					.FirstOrDefaultAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve verification code from the database.", ex);
			}
		}

		public async Task<int> CountCreatedSinceAsync(string channel, string target, DateTime since)
		{
			try
			{
				return await dbContext.VerificationCodes
					.CountAsync(c => c.Channel == channel && c.Target == target && c.CreatedAt >= since);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to count verification codes in the database.", ex);
			}
		}

		public async Task IncrementAttemptsAsync(int codeId)
		{
			try
			{
				await dbContext.VerificationCodes
					.Where(c => c.Id == codeId)
					.ExecuteUpdateAsync(setters => setters.SetProperty(c => c.AttemptCount, c => c.AttemptCount + 1));
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update verification code in the database.", ex);
			}
		}

		public async Task<bool> ConsumeAsync(int codeId)
		{
			try
			{
				var affected = await dbContext.VerificationCodes
					.Where(c => c.Id == codeId && c.ConsumedAt == null)
					.ExecuteUpdateAsync(setters => setters.SetProperty(c => c.ConsumedAt, DateTime.UtcNow));

				return affected > 0;
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update verification code in the database.", ex);
			}
		}
	}
}

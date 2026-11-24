using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class RefreshTokenRepository(IVocabularyTrainerDbContext dbContext) : IRefreshTokenRepository
	{
		public async Task<RefreshTokenInfo?> GetByHashAsync(byte[] tokenHash)
		{
			try
			{
				return await dbContext.RefreshTokens
					.AsNoTracking()
					.Where(t => t.TokenHash == tokenHash)
					.Select(t => new RefreshTokenInfo(t.Id, t.UserId, t.ClientType, t.DeviceName, t.ExpiresAt, t.RevokedAt, t.ReplacedById))
					.SingleOrDefaultAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve refresh token from the database.", ex);
			}
		}

		public async Task<int> AddAsync(NewRefreshToken token)
		{
			var entity = new RefreshToken
			{
				UserId = token.UserId,
				TokenHash = token.TokenHash,
				ClientType = token.ClientType,
				DeviceName = token.DeviceName,
				CreatedAt = token.CreatedAt,
				ExpiresAt = token.ExpiresAt,
			};

			try
			{
				dbContext.RefreshTokens.Add(entity);
				await dbContext.SaveChangesAsync();
				return entity.Id;
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to add refresh token to the database.", ex);
			}
		}

		public async Task<bool> RotateAsync(int oldTokenId, NewRefreshToken replacement)
		{
			int revoked;

			try
			{
				// Compare-and-swap on RevokedAt: of two parallel refreshes with the same token only one wins.
				revoked = await dbContext.RefreshTokens
					.Where(t => t.Id == oldTokenId && t.RevokedAt == null)
					.ExecuteUpdateAsync(setters => setters.SetProperty(t => t.RevokedAt, DateTime.UtcNow));
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to rotate refresh token in the database.", ex);
			}

			if (revoked == 0) return false;

			var newTokenId = await AddAsync(replacement);

			try
			{
				await dbContext.RefreshTokens
					.Where(t => t.Id == oldTokenId)
					.ExecuteUpdateAsync(setters => setters.SetProperty(t => t.ReplacedById, newTokenId));
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to rotate refresh token in the database.", ex);
			}

			return true;
		}

		public async Task RevokeAsync(int tokenId)
		{
			try
			{
				await dbContext.RefreshTokens
					.Where(t => t.Id == tokenId && t.RevokedAt == null)
					.ExecuteUpdateAsync(setters => setters.SetProperty(t => t.RevokedAt, DateTime.UtcNow));
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to revoke refresh token in the database.", ex);
			}
		}

		public async Task<int> DeleteExpiredBeforeAsync(DateTime threshold)
		{
			try
			{
				return await dbContext.RefreshTokens
					.Where(t => t.ExpiresAt < threshold)
					.ExecuteDeleteAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to delete expired refresh tokens from the database.", ex);
			}
		}

		public async Task RevokeAllForUserAsync(int userId, byte[]? exceptTokenHash = null)
		{
			try
			{
				await dbContext.RefreshTokens
					.Where(t => t.UserId == userId && t.RevokedAt == null)
					.Where(t => exceptTokenHash == null || t.TokenHash != exceptTokenHash)
					.ExecuteUpdateAsync(setters => setters.SetProperty(t => t.RevokedAt, DateTime.UtcNow));
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to revoke refresh tokens in the database.", ex);
			}
		}
	}
}

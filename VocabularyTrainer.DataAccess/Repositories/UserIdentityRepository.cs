using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class UserIdentityRepository(IVocabularyTrainerDbContext dbContext) : IUserIdentityRepository
	{
		public async Task<List<UserIdentityDto>> GetByUserAsync(int userId)
		{
			try
			{
				return await dbContext.UserIdentities
					.AsNoTracking()
					.Where(i => i.UserId == userId)
					.OrderBy(i => i.CreatedAt)
					.Select(i => new UserIdentityDto(i.Id, i.Provider, i.ProviderKey, i.CreatedAt, i.LastLoginAt))
					.ToListAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve user identities from the database.", ex);
			}
		}

		public async Task<bool> ExistsAsync(string provider, string providerKey)
		{
			try
			{
				return await dbContext.UserIdentities
					.AnyAsync(i => i.Provider == provider && i.ProviderKey == providerKey);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to check user identity in the database.", ex);
			}
		}

		public async Task<int> AddAsync(int userId, string provider, string providerKey)
		{
			var entity = new UserIdentity
			{
				UserId = userId,
				Provider = provider,
				ProviderKey = providerKey,
				CreatedAt = DateTime.UtcNow,
			};

			try
			{
				dbContext.UserIdentities.Add(entity);
				await dbContext.SaveChangesAsync();
				return entity.Id;
			}
			catch (DbUpdateException ex) when (SqlServerDbErrors.IsUniqueViolation(ex))
			{
				throw new DuplicateKeyException("This sign-in identity is already registered.", ex);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to add user identity to the database.", ex);
			}
		}

		public async Task<int> DeleteUnlessLastAsync(int identityId, int userId)
		{
			try
			{
				// Check and delete in one statement so two concurrent unlinks cannot both see "2 left".
				// Raw SQL only for the lock hints: UPDLOCK + HOLDLOCK make the count take update locks on the
				// user's rows, so a second unlink waits for the first to commit and then sees the real count,
				// regardless of READ_COMMITTED_SNAPSHOT (on by default in Azure SQL).
				return await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
					DELETE FROM dbo.UserIdentities
					WHERE ID = {identityId}
					  AND UserId = {userId}
					  AND (SELECT COUNT(*) FROM dbo.UserIdentities WITH (UPDLOCK, HOLDLOCK) WHERE UserId = {userId}) > 1
					""");
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to delete user identity from the database.", ex);
			}
		}

		public async Task TouchLastLoginAsync(string provider, string providerKey)
		{
			try
			{
				await dbContext.UserIdentities
					.Where(i => i.Provider == provider && i.ProviderKey == providerKey)
					.ExecuteUpdateAsync(setters => setters.SetProperty(i => i.LastLoginAt, DateTime.UtcNow));
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update user identity in the database.", ex);
			}
		}
	}
}

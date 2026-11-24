using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class UserRepository(IVocabularyTrainerDbContext dbContext) : IUserRepository
	{
		public async Task<UserModel?> GetByIdAsync(int userId)
		{
			try
			{
				return await dbContext.Users
					.AsNoTracking()
					.Where(u => u.Id == userId)
					.Select(u => new UserModel(u.Id, u.DisplayName))
					.SingleOrDefaultAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve user from the database.", ex);
			}
		}

		public async Task<UserCredentials?> GetCredentialsByIdAsync(int userId)
		{
			try
			{
				return await dbContext.Users
					.AsNoTracking()
					.Where(u => u.Id == userId)
					.Select(u => new UserCredentials(u.Id, u.DisplayName, u.PasswordHash, u.IsBlocked))
					.SingleOrDefaultAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve user from the database.", ex);
			}
		}

		public async Task<UserCredentials?> GetCredentialsByIdentityAsync(string provider, string providerKey)
		{
			try
			{
				return await dbContext.UserIdentities
					.AsNoTracking()
					.Where(i => i.Provider == provider && i.ProviderKey == providerKey)
					.Select(i => new UserCredentials(i.User.Id, i.User.DisplayName, i.User.PasswordHash, i.User.IsBlocked))
					.SingleOrDefaultAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve user from the database.", ex);
			}
		}

		public async Task<int> CreateWithIdentityAsync(string displayName, string? passwordHash, string provider, string providerKey)
		{
			var now = DateTime.UtcNow;

			var identity = new UserIdentity
			{
				Provider = provider,
				ProviderKey = providerKey,
				CreatedAt = now,
				LastLoginAt = now,
			};

			var user = new User
			{
				DisplayName = displayName,
				PasswordHash = passwordHash,
				CreatedAt = now,
				Identities = [identity],
			};

			try
			{
				dbContext.Users.Add(user);
				await dbContext.SaveChangesAsync();
				return user.Id;
			}
			catch (DbUpdateException ex) when (SqlServerDbErrors.IsUniqueViolation(ex))
			{
				throw new DuplicateKeyException("This sign-in identity is already registered.", ex);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to create user in the database.", ex);
			}
		}

		public async Task UpdatePasswordHashAsync(int userId, string passwordHash)
		{
			try
			{
				await dbContext.Users
					.Where(u => u.Id == userId)
					.ExecuteUpdateAsync(setters => setters.SetProperty(u => u.PasswordHash, passwordHash));
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update user password in the database.", ex);
			}
		}

		public async Task<int> UpdateDisplayNameAsync(int userId, string displayName)
		{
			try
			{
				return await dbContext.Users
					.Where(u => u.Id == userId)
					.ExecuteUpdateAsync(setters => setters.SetProperty(u => u.DisplayName, displayName));
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update user name in the database.", ex);
			}
		}
	}
}

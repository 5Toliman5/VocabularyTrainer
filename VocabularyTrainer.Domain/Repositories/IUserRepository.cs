using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IUserRepository
	{
		Task<UserModel?> GetByIdAsync(int userId);

		Task<UserCredentials?> GetCredentialsByIdAsync(int userId);

		Task<UserCredentials?> GetCredentialsByIdentityAsync(string provider, string providerKey);

		// Creates the account and its first identity atomically. Throws DuplicateKeyException if the identity is taken.
		Task<int> CreateWithIdentityAsync(string displayName, string? passwordHash, string provider, string providerKey);

		Task UpdatePasswordHashAsync(int userId, string passwordHash);

		Task<int> UpdateDisplayNameAsync(int userId, string displayName);
	}
}

using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IUserIdentityRepository
	{
		Task<List<UserIdentityDto>> GetByUserAsync(int userId);

		Task<bool> ExistsAsync(string provider, string providerKey);

		// Throws DuplicateKeyException if the identity already belongs to any account.
		Task<int> AddAsync(int userId, string provider, string providerKey);

		// Deletes only if the user keeps at least one other identity; returns rows affected (0 when it was the last one).
		Task<int> DeleteUnlessLastAsync(int identityId, int userId);

		Task TouchLastLoginAsync(string provider, string providerKey);
	}
}

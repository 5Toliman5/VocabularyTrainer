using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	/// <summary>Persistence contract for user lookup operations.</summary>
	public interface IUserRepository
	{
		/// <summary>Returns a lightweight user projection by name, or <see langword="null"/> if not found.</summary>
		Task<UserModel?> GetUserAsync(string userName);
	}
}

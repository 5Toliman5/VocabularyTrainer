using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	/// <summary>Business logic contract for user lookup.</summary>
	public interface IUserService
	{
		/// <summary>Returns a lightweight user projection by name, or <see langword="null"/> if not found.</summary>
		Task<UserModel?> GetAsync(string userName);
	}
}

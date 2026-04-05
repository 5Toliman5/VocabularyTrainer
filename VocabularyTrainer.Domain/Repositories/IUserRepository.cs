using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IUserRepository
	{
		Task<UserModel?> GetUserAsync(string userName);
	}
}
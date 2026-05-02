using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IUserRepository
	{
		Task<Result<UserModel>> GetUserAsync(string userName);
	}
}

using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IUserService
	{
		Task<Result<UserModel>> GetAsync(string userName);
	}
}

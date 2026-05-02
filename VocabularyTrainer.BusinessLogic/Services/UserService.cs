using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class UserService(IUserRepository repository) : IUserService
	{
		public Task<Result<UserModel>> GetAsync(string userName)
		{
			return repository.GetUserAsync(userName);
		}
	}
}

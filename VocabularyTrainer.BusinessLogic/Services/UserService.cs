using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
    public class UserService(IUserRepository repository) : IUserService
	{
		public Task<UserModel?> GetAsync(string userName) => repository.GetUserAsync(userName);
	}
}

using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	/// <summary>Implements user lookup business logic.</summary>
	public class UserService(IUserRepository repository) : IUserService
	{
        /// <inheritdoc/>
        public Task<UserModel?> GetAsync(string userName)
        {
            return repository.GetUserAsync(userName);
        }
    }
}

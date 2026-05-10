using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class UserService(IUserRepository userRepository) : IUserService
	{
		public async Task<Result<UserModel>> GetAsync(string userName)
		{
			var user = await userRepository.GetUserAsync(userName);
			return user is not null
				? Result<UserModel>.Success(user)
				: Result<UserModel>.Failure($"User '{userName}' was not found.", ResultErrorKind.NotFound);
		}
	}
}

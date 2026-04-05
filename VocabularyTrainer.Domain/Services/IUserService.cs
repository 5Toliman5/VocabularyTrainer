using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
    public interface IUserService
    {
		Task<UserModel?> GetAsync(string userName);
	}
}
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Services
{
	public interface ITokenService
	{
		AccessToken CreateAccessToken(UserModel user);
	}
}

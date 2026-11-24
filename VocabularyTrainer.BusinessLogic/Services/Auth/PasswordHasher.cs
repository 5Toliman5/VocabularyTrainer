using Microsoft.AspNetCore.Identity;
using VocabularyTrainer.Domain.Services;
using AspNetPasswordHasher = Microsoft.AspNetCore.Identity.PasswordHasher<object>;

namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	// Only the PBKDF2 hasher from ASP.NET Core Identity is used; Identity stores/tables are not.
	public class PasswordHasher : IPasswordHasher
	{
		private static readonly object NoUser = new();
		private readonly AspNetPasswordHasher _hasher = new();

		public string Hash(string password) => _hasher.HashPassword(NoUser, password);

		public PasswordCheck Verify(string passwordHash, string password)
		{
			return _hasher.VerifyHashedPassword(NoUser, passwordHash, password) switch
			{
				PasswordVerificationResult.Success => PasswordCheck.Success,
				PasswordVerificationResult.SuccessRehashNeeded => PasswordCheck.SuccessRehashNeeded,
				_ => PasswordCheck.Failed,
			};
		}
	}
}

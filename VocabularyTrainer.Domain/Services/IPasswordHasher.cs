namespace VocabularyTrainer.Domain.Services
{
	public enum PasswordCheck
	{
		Failed,
		Success,
		SuccessRehashNeeded,
	}

	public interface IPasswordHasher
	{
		string Hash(string password);

		PasswordCheck Verify(string passwordHash, string password);
	}
}

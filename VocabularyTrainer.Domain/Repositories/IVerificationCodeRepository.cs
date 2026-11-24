using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IVerificationCodeRepository
	{
		Task<int> AddAsync(NewVerificationCode code);

		Task DeleteAsync(int codeId);

		Task<int> DeleteExpiredBeforeAsync(DateTime threshold);

		Task<VerificationCodeInfo?> GetLatestAsync(string channel, string target, VerificationPurpose purpose);

		Task<int> CountCreatedSinceAsync(string channel, string target, DateTime since);

		Task IncrementAttemptsAsync(int codeId);

		// Returns false if the code was already consumed (concurrent verification).
		Task<bool> ConsumeAsync(int codeId);
	}
}

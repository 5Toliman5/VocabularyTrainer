namespace VocabularyTrainer.Domain.Models.Auth
{
	public record VerificationCodeInfo
	(
		int Id,
		byte[] CodeHash,
		DateTime CreatedAt,
		DateTime ExpiresAt,
		int AttemptCount,
		DateTime? ConsumedAt
	);

	public record NewVerificationCode
	(
		string Channel,
		string Target,
		VerificationPurpose Purpose,
		byte[] CodeHash,
		DateTime CreatedAt,
		DateTime ExpiresAt
	);
}

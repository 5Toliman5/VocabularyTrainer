namespace VocabularyTrainer.Domain.Models.Auth
{
	public record AuthPurgeResult
	(
		int VerificationCodes,
		int RefreshTokens
	);
}

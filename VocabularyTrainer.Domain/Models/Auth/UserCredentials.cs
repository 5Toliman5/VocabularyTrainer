namespace VocabularyTrainer.Domain.Models.Auth
{
	public record UserCredentials
	(
		int UserId,
		string DisplayName,
		string? PasswordHash,
		bool IsBlocked
	);
}

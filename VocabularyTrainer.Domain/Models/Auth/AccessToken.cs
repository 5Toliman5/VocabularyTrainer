namespace VocabularyTrainer.Domain.Models.Auth
{
	public record AccessToken
	(
		string Value,
		DateTime ExpiresAt
	);
}

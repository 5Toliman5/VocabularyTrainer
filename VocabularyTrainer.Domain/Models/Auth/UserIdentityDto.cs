namespace VocabularyTrainer.Domain.Models.Auth
{
	public record UserIdentityDto
	(
		int Id,
		string Provider,
		string ProviderKey,
		DateTime CreatedAt,
		DateTime? LastLoginAt
	);
}

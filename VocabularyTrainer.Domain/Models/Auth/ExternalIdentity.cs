namespace VocabularyTrainer.Domain.Models.Auth
{
	public record ExternalIdentity
	(
		string Provider,
		string ProviderKey,
		string? DisplayName
	);
}

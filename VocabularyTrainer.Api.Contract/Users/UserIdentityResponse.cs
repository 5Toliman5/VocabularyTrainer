namespace VocabularyTrainer.Api.Contract.Users
{
	public record UserIdentityResponse
	(
		int Id,
		string Provider,
		string ProviderKey,
		DateTime CreatedAt,
		DateTime? LastLoginAt
	);
}

namespace VocabularyTrainer.Api.Contract.Users
{
	public record LinkIdentityRequest
	(
		string Channel,
		string Target,
		string Code
	);
}

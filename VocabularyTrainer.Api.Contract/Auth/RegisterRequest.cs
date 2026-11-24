namespace VocabularyTrainer.Api.Contract.Auth
{
	public record RegisterRequest
	(
		string Channel,
		string Target,
		string Code,
		string Password,
		string DisplayName,
		ClientInfoDto Client
	);
}

namespace VocabularyTrainer.Api.Contract.Auth
{
	// Login is an email or a phone number in international format.
	public record LoginRequest
	(
		string Login,
		string Password,
		ClientInfoDto Client
	);
}

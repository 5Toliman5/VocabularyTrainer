namespace VocabularyTrainer.Api.Contract.Auth
{
	// Credential is whatever the provider issues: an id_token for Google/Apple, initData for Telegram, ...
	public record ExternalLoginRequest
	(
		string Credential,
		string? DisplayNameHint,
		ClientInfoDto Client
	);
}

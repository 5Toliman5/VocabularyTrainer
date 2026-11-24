namespace VocabularyTrainer.Api.Contract.Auth
{
	public record ResetPasswordRequest
	(
		string Channel,
		string Target,
		string Code,
		string NewPassword
	);
}

namespace VocabularyTrainer.Api.Contract.Auth
{
	// Channel is an identity provider code: "email" now, "phone" once a phone delivery channel exists.
	public record SendCodeRequest
	(
		string Channel,
		string Target,
		VerificationPurpose Purpose
	);
}

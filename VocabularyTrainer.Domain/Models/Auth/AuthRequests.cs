namespace VocabularyTrainer.Domain.Models.Auth
{
	public record SendVerificationCodeRequest
	(
		string Channel,
		string Target,
		VerificationPurpose Purpose
	);

	public record RegisterRequest
	(
		string Channel,
		string Target,
		string Code,
		string Password,
		string DisplayName,
		ClientInfo Client
	);

	public record PasswordLoginRequest
	(
		string Login,
		string Password,
		ClientInfo Client
	);

	public record ResetPasswordRequest
	(
		string Channel,
		string Target,
		string Code,
		string NewPassword
	);

	public record ExternalLoginRequest
	(
		string Provider,
		string Credential,
		string? DisplayNameHint,
		ClientInfo Client
	);

	// CurrentRefreshToken identifies the caller's own session, which stays signed in; all other sessions are revoked.
	public record ChangePasswordRequest
	(
		int UserId,
		string? CurrentPassword,
		string NewPassword,
		string? CurrentRefreshToken
	);

	public record LinkIdentityRequest
	(
		int UserId,
		string Channel,
		string Target,
		string Code
	);
}

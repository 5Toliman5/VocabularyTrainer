namespace VocabularyTrainer.Api.Contract.Users
{
	// CurrentRefreshToken: the caller's own session stays signed in; every other session is revoked.
	// CurrentPassword may be empty only for accounts that have no password yet.
	public record ChangePasswordRequest
	(
		string? CurrentPassword,
		string NewPassword,
		string? CurrentRefreshToken
	);
}

namespace VocabularyTrainer.Domain.Models.Auth
{
	public record AuthTokens
	(
		string AccessToken,
		DateTime AccessTokenExpiresAt,
		string RefreshToken,
		DateTime RefreshTokenExpiresAt,
		UserModel User
	);
}

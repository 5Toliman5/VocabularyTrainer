using VocabularyTrainer.Api.Contract.Users;

namespace VocabularyTrainer.Api.Contract.Auth
{
	public record AuthTokensResponse
	(
		string AccessToken,
		DateTime AccessTokenExpiresAt,
		string RefreshToken,
		DateTime RefreshTokenExpiresAt,
		UserResponse User
	);
}

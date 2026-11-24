namespace VocabularyTrainer.Domain.Models.Auth
{
	public record RefreshTokenInfo
	(
		int Id,
		int UserId,
		string ClientType,
		string? DeviceName,
		DateTime ExpiresAt,
		DateTime? RevokedAt,
		int? ReplacedById
	);

	public record NewRefreshToken
	(
		int UserId,
		byte[] TokenHash,
		string ClientType,
		string? DeviceName,
		DateTime CreatedAt,
		DateTime ExpiresAt
	);
}

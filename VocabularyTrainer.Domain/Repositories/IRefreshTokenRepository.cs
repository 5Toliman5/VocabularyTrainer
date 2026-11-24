using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Repositories
{
	public interface IRefreshTokenRepository
	{
		Task<RefreshTokenInfo?> GetByHashAsync(byte[] tokenHash);

		Task<int> AddAsync(NewRefreshToken token);

		// Returns false if the old token was revoked concurrently (another refresh won the race).
		Task<bool> RotateAsync(int oldTokenId, NewRefreshToken replacement);

		Task RevokeAsync(int tokenId);

		Task RevokeAllForUserAsync(int userId, byte[]? exceptTokenHash = null);
	}
}

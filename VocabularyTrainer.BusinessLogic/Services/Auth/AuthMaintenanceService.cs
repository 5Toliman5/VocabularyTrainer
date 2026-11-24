using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	public class AuthMaintenanceService(
		IVerificationCodeRepository verificationCodeRepository,
		IRefreshTokenRepository refreshTokenRepository) : IAuthMaintenanceService
	{
		// Rows are kept a day past expiry: a rotated refresh token must still be found while it could be replayed
		// (reuse detection), and the per-hour code limit counts recent rows.
		private static readonly TimeSpan RetentionAfterExpiry = TimeSpan.FromDays(1);

		public async Task<AuthPurgeResult> PurgeExpiredAsync()
		{
			var threshold = DateTime.UtcNow - RetentionAfterExpiry;

			var codes = await verificationCodeRepository.DeleteExpiredBeforeAsync(threshold);
			var tokens = await refreshTokenRepository.DeleteExpiredBeforeAsync(threshold);

			return new AuthPurgeResult(codes, tokens);
		}
	}
}

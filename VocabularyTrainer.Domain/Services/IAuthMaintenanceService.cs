using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Services
{
	public interface IAuthMaintenanceService
	{
		// Deletes verification codes and refresh tokens that can no longer be used.
		Task<AuthPurgeResult> PurgeExpiredAsync();
	}
}

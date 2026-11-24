using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.Infrastructure.Auth
{
	// Runs right after startup and then periodically; the work itself lives in IAuthMaintenanceService.
	public class AuthCleanupHostedService(IServiceScopeFactory scopeFactory, ILogger<AuthCleanupHostedService> logger)
		: BackgroundService
	{
		private static readonly TimeSpan Interval = TimeSpan.FromHours(6);

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var timer = new PeriodicTimer(Interval);

			do
			{
				await PurgeOnceAsync();
			}
			while (await timer.WaitForNextTickAsync(stoppingToken));
		}

		private async Task PurgeOnceAsync()
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var maintenance = scope.ServiceProvider.GetRequiredService<IAuthMaintenanceService>();
				var result = await maintenance.PurgeExpiredAsync();

				logger.LogInformation(
					"Auth cleanup removed {Codes} verification codes and {Tokens} refresh tokens",
					result.VerificationCodes,
					result.RefreshTokens
				);
			}
			catch (Exception ex)
			{
				// A failed cleanup must not stop the host; the next tick retries.
				logger.LogError(ex, "Auth cleanup failed");
			}
		}
	}
}

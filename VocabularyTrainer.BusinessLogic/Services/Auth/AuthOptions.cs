namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	public class AuthOptions
	{
		public const string SectionName = "Auth";

		public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(60);

		public TimeSpan VerificationCodeLifetime { get; set; } = TimeSpan.FromMinutes(10);

		public TimeSpan VerificationCodeResendCooldown { get; set; } = TimeSpan.FromSeconds(60);

		public int VerificationCodesPerHour { get; set; } = 5;

		public int VerificationCodeMaxAttempts { get; set; } = 5;

		// Per client IP on the credential endpoints (codes, register, login, password reset); refresh/logout are not limited.
		public int AuthRequestsPerMinutePerIp { get; set; } = 30;
	}
}

namespace VocabularyTrainer.Domain.Models.Auth
{
	// Stable string codes persisted in UserIdentities.Provider and VerificationCodes.Channel.
	public static class IdentityProviders
	{
		public const string Email = "email";
		public const string Phone = "phone";
		public const string Telegram = "telegram";
		public const string Google = "google";
		public const string Apple = "apple";
		public const string Facebook = "facebook";
	}
}

using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Services
{
	// One implementation per delivery channel (email now; Telegram/Viber for phone numbers later).
	public interface IVerificationCodeSender
	{
		string Channel { get; }

		Task SendAsync(string target, string code, VerificationPurpose purpose, TimeSpan lifetime);
	}
}

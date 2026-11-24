using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Services
{
	public interface IEmailSender
	{
		Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
	}
}

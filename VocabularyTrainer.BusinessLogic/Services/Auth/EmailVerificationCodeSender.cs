using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	public class EmailVerificationCodeSender(IEmailSender emailSender) : IVerificationCodeSender
	{
		public string Channel => IdentityProviders.Email;

		public Task SendAsync(string target, string code, VerificationPurpose purpose, TimeSpan lifetime)
		{
			var action = purpose switch
			{
				VerificationPurpose.Register => "finish creating your account",
				VerificationPurpose.ResetPassword => "reset your password",
				VerificationPurpose.LinkIdentity => "add this email to your account",
				_ => "confirm your email",
			};

			var minutes = (int)Math.Ceiling(lifetime.TotalMinutes);

			var text =
				$"Your Vocabulary Trainer code: {code}\n\n" +
				$"Enter it to {action}. The code expires in {minutes} minutes.\n\n" +
				"If you did not request this code, just ignore this email.";

			var html =
				$"<p>Your Vocabulary Trainer code:</p>" +
				$"<p style=\"font-size:28px;font-weight:bold;letter-spacing:4px\">{code}</p>" +
				$"<p>Enter it to {action}. The code expires in {minutes} minutes.</p>" +
				"<p style=\"color:#777\">If you did not request this code, just ignore this email.</p>";

			var message = new EmailMessage(target, $"{code} is your Vocabulary Trainer code", text, html);
			return emailSender.SendAsync(message);
		}
	}
}

namespace VocabularyTrainer.Infrastructure.Email
{
	public enum SmtpSecurity
	{
		Auto,
		None,
		StartTls,
		SslOnConnect,
	}

	// Any SMTP relay works (Mailpit locally; Brevo, Mailjet, Resend, Amazon SES, ... in production).
	public class SmtpOptions
	{
		public const string SectionName = "Smtp";

		public string Host { get; set; } = "localhost";

		public int Port { get; set; } = 1025;

		public SmtpSecurity Security { get; set; } = SmtpSecurity.Auto;

		public string? UserName { get; set; }

		public string? Password { get; set; }

		public string FromAddress { get; set; } = "no-reply@vocabularytrainer.local";

		public string FromName { get; set; } = "Vocabulary Trainer";
	}
}

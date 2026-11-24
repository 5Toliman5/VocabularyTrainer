using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Infrastructure.Email
{
	public class SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
	{
		private readonly SmtpOptions _options = options.Value;

		public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
		{
			var mime = BuildMimeMessage(message);

			using var client = new SmtpClient();

			await client.ConnectAsync(_options.Host, _options.Port, ToSecureSocketOptions(_options.Security), cancellationToken);

			if (!string.IsNullOrEmpty(_options.UserName))
			{
				await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
			}

			await client.SendAsync(new SendRequest(mime), cancellationToken);
			await client.DisconnectAsync(quit: true, cancellationToken);

			logger.LogInformation("Email '{Subject}' sent via {Host}:{Port}", message.Subject, _options.Host, _options.Port);
		}

		private MimeMessage BuildMimeMessage(EmailMessage message)
		{
			var mime = new MimeMessage();
			mime.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
			mime.To.Add(MailboxAddress.Parse(message.To));
			mime.Subject = message.Subject;

			var body = new BodyBuilder
			{
				TextBody = message.TextBody,
				HtmlBody = message.HtmlBody,
			};

			mime.Body = body.ToMessageBody();
			return mime;
		}

		private static SecureSocketOptions ToSecureSocketOptions(SmtpSecurity security)
		{
			return security switch
			{
				SmtpSecurity.None => SecureSocketOptions.None,
				SmtpSecurity.StartTls => SecureSocketOptions.StartTls,
				SmtpSecurity.SslOnConnect => SecureSocketOptions.SslOnConnect,
				_ => SecureSocketOptions.Auto,
			};
		}
	}
}

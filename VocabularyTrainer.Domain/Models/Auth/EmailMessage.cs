namespace VocabularyTrainer.Domain.Models.Auth
{
	public record EmailMessage
	(
		string To,
		string Subject,
		string TextBody,
		string? HtmlBody
	);
}

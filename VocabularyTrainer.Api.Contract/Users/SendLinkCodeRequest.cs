namespace VocabularyTrainer.Api.Contract.Users
{
	public record SendLinkCodeRequest
	(
		string Channel,
		string Target
	);
}

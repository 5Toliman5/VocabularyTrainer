namespace VocabularyTrainer.Domain.Models.Auth
{
	public record ClientInfo
	(
		string ClientType,
		string? DeviceName
	);
}

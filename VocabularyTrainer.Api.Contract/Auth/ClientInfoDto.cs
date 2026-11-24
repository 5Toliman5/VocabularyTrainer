namespace VocabularyTrainer.Api.Contract.Auth
{
	// ClientType identifies the front-end ("WinApp", "Telegram", "iOS", ...); DeviceName is shown in a future sessions list.
	public record ClientInfoDto
	(
		string ClientType,
		string? DeviceName
	);
}

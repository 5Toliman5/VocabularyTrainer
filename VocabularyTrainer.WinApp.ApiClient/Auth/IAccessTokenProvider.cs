namespace VocabularyTrainer.WinApp.ApiClient.Auth
{
	internal interface IAccessTokenProvider
	{
		// Null when there is no signed-in session.
		Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken);

		// Called after the API rejected rejectedToken. Null when the session cannot be renewed.
		Task<string?> RefreshAccessTokenAsync(string? rejectedToken, CancellationToken cancellationToken);
	}
}

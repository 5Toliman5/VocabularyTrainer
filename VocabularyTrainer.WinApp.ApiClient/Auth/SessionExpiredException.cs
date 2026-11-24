namespace VocabularyTrainer.WinApp.ApiClient.Auth
{
	public sealed class SessionExpiredException() : Exception("The session has expired. Please sign in again.")
	{
	}
}

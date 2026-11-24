namespace VocabularyTrainer.WinApp.ApiClient.Auth
{
	// Persists the refresh token between app launches. The host decides how (DPAPI on Windows, Keychain on macOS, ...).
	public interface ITokenStore
	{
		string? Load();

		void Save(string refreshToken);

		void Clear();
	}
}

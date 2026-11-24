using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		event EventHandler? LogoutRequested;

		void ShowError(string message);
		void LoadDictionaries(IReadOnlyList<DictionaryDto> dictionaries);
		void SetCurrentUser(string displayName);

		// Restarting the process sends the user back through the startup sign-in flow.
		void RestartApplication();
	}
}

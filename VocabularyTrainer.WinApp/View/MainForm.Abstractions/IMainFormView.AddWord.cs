namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		event EventHandler? AddWordRequested;

		string InputWord { get; }
		string InputTranslation { get; }
		int? SelectedAddingDictionaryId { get; }

		bool ValidateAddWordInput();
		void ClearAddWordInput();
		void ShowAddingDictionaryError(string message);
	}
}

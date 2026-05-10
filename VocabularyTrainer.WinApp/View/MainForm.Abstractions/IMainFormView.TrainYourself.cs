namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		event EventHandler<string> UserChanged;
		event EventHandler<int?> TrainingDictionaryChanged;
		event EventHandler? DeleteWordRequested;
		event EventHandler? ShowNextWordRequested;
		event EventHandler? ShowTranslationRequested;

		string CurrentUserName { get; }

		void ClearShowWordOutput();
		void DisplayNewWord(string word);
		void DisplayTranslation(string translation);
		void SetCurrentWordDictionary(string dictName);
		void SetShowNextButtonText(string text);
	}
}

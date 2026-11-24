namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		event EventHandler TrainingPageEntered;
		event EventHandler<int?> TrainingDictionaryChanged;
		event EventHandler? DeleteWordRequested;
		event EventHandler? ShowNextWordRequested;
		event EventHandler? ShowTranslationRequested;

		void ClearShowWordOutput();
		void DisplayNewWord(string word);
		void DisplayTranslation(string translation);
		void SetCurrentWordDictionary(string dictName);
		void SetShowNextButtonText(string text);
	}
}

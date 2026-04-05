using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.View
{
	public interface IMainFormView
	{
		// --- Events ---
		event EventHandler<string> UserChanged;
		event EventHandler<int?> TrainingDictionaryChanged;
		event EventHandler? AddWordRequested;
		event EventHandler? DeleteWordRequested;
		event EventHandler? ShowNextWordRequested;
		event EventHandler? ShowTranslationRequested;
		event EventHandler? AddDictionaryRequested;
		event EventHandler? UpdateDictionaryRequested;
		event EventHandler? DeleteDictionaryRequested;
		event EventHandler? MyWordsPageEntered;

		// --- Properties ---
		string CurrentUserName { get; }
		string InputWord { get; }
		string InputTranslation { get; }
		int? SelectedAddingDictionaryId { get; }
		int? SelectedMyWordsDictionaryId { get; }
		string InputDictionaryName { get; }
		string? InputLanguageCode { get; }

		// --- Word display ---
		void ClearAddWordInput();
		void ClearShowWordOutput();
		void DisplayNewWord(string word);
		void DisplayTranslation(string translation);
		void SetCurrentWordDictionary(string dictName);
		void SetShowNextButtonText(string text);
		void ShowError(string message);
		bool ValidateAddWordInput();

		// --- Dictionary management ---
		void LoadDictionaries(IReadOnlyList<DictionaryDto> dicts);
		void ShowAddingDictionaryError(string message);
		void ClearMyWordsDictionaryInput();
	}
}

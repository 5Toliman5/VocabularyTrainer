using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.View
{
	public interface IMainFormView
	{
		event EventHandler<string> UserChanged;
		event EventHandler<int?> TrainingDictionaryChanged;
		event EventHandler? AddWordRequested;
		event EventHandler? DeleteWordRequested;
		event EventHandler? ShowNextWordRequested;
		event EventHandler? ShowTranslationRequested;
		event EventHandler<DictionaryInputEventArgs>? AddDictionaryRequested;
		event EventHandler? UpdateDictionaryRequested;
		event EventHandler? DeleteDictionaryRequested;
		event EventHandler? MyDictionariesPageEntered;
		event EventHandler? MyWordsPageEntered;
		event EventHandler? ApplyWordFilterRequested;
		event EventHandler? PreviousWordPageRequested;
		event EventHandler? NextWordPageRequested;
		event EventHandler? DeleteMyWordsWordRequested;
		event EventHandler<string>? MyWordsSortChanged;
		event EventHandler? ResetWordFilterRequested;

		string CurrentUserName { get; }
		string InputWord { get; }
		string InputTranslation { get; }
		int? SelectedAddingDictionaryId { get; }
		int? SelectedMyWordsDictionaryId { get; }
		string InputDictionaryName { get; }
		string? InputLanguageCode { get; }

		int? MyWordsDictionaryId { get; }
		string? MyWordsLanguage { get; }
		string? MyWordsSearch { get; }
		DateTime? MyWordsDateFrom { get; }
		DateTime? MyWordsDateTo { get; }
		WordDto? SelectedMyWordsWord { get; }

		bool ValidateAddWordInput();
		void ClearAddWordInput();
		void ClearShowWordOutput();
		void DisplayNewWord(string word);
		void DisplayTranslation(string translation);
		void SetCurrentWordDictionary(string dictName);
		void SetShowNextButtonText(string text);
		void ShowError(string message);

		void LoadDictionaries(IReadOnlyList<DictionaryDto> dicts);
		void ShowAddingDictionaryError(string message);
		void ClearMyWordsDictionaryInput();

		void LoadMyWordsPage(PagedResult<WordDto> result);
		void ResetMyWordsFilters();
	}
}

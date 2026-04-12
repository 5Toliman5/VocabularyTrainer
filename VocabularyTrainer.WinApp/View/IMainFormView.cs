using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.View
{
	/// <summary>View contract for the main application form, used by <see cref="VocabularyTrainer.WinApp.Presenter.MainFormPresenter"/>.</summary>
	public interface IMainFormView
	{
		// --- Events ---

		/// <summary>Raised when the user changes the active username.</summary>
		event EventHandler<string> UserChanged;

		/// <summary>Raised when the user selects a different training dictionary. The argument is the dictionary ID, or <see langword="null"/> for all dictionaries.</summary>
		event EventHandler<int?> TrainingDictionaryChanged;

		/// <summary>Raised when the user requests to add the current word input to their dictionary.</summary>
		event EventHandler? AddWordRequested;

		/// <summary>Raised when the user requests to delete the current word.</summary>
		event EventHandler? DeleteWordRequested;

		/// <summary>Raised when the user requests the next word.</summary>
		event EventHandler? ShowNextWordRequested;

		/// <summary>Raised when the user requests to reveal the translation of the current word.</summary>
		event EventHandler? ShowTranslationRequested;

		/// <summary>Raised when the user requests to add a new dictionary.</summary>
		event EventHandler? AddDictionaryRequested;

		/// <summary>Raised when the user requests to update the selected dictionary.</summary>
		event EventHandler? UpdateDictionaryRequested;

		/// <summary>Raised when the user requests to delete the selected dictionary.</summary>
		event EventHandler? DeleteDictionaryRequested;

		/// <summary>Raised when the user navigates to the My Words page.</summary>
		event EventHandler? MyWordsPageEntered;

		// --- Properties ---

		/// <summary>The username currently entered in the login field.</summary>
		string CurrentUserName { get; }

		/// <summary>The word text currently entered in the add-word input.</summary>
		string InputWord { get; }

		/// <summary>The translation text currently entered in the add-word input.</summary>
		string InputTranslation { get; }

		/// <summary>The dictionary selected in the add-word combo box, or <see langword="null"/> if none is selected.</summary>
		int? SelectedAddingDictionaryId { get; }

		/// <summary>The dictionary selected in the My Words list, or <see langword="null"/> if none is selected.</summary>
		int? SelectedMyWordsDictionaryId { get; }

		/// <summary>The dictionary name currently entered in the dictionary name input.</summary>
		string InputDictionaryName { get; }

		/// <summary>The BCP 47 language code resolved from the language combo box selection, or <see langword="null"/> if empty.</summary>
		string? InputLanguageCode { get; }

		// --- Word display ---

		/// <summary>Validates the word and translation inputs and sets error provider messages for empty fields.</summary>
		bool ValidateAddWordInput();

		/// <summary>Clears the word and translation input fields and resets their error state.</summary>
		void ClearAddWordInput();

		/// <summary>Clears the word, translation, and dictionary label in the training display.</summary>
		void ClearShowWordOutput();

		/// <summary>Displays the given word text in the training area.</summary>
		void DisplayNewWord(string word);

		/// <summary>Displays the given translation text in the training area.</summary>
		void DisplayTranslation(string translation);

		/// <summary>Sets the dictionary label shown beneath the current word.</summary>
		void SetCurrentWordDictionary(string dictName);

		/// <summary>Sets the label text on the Show Next button.</summary>
		void SetShowNextButtonText(string text);

		/// <summary>Shows a modal error dialog with the given message.</summary>
		void ShowError(string message);

		// --- Dictionary management ---

		/// <summary>Repopulates all dictionary controls (training combo, adding combo, My Words list) from the provided list.</summary>
		void LoadDictionaries(IReadOnlyList<DictionaryDto> dicts);

		/// <summary>Sets an error indicator on the adding-dictionary combo box.</summary>
		void ShowAddingDictionaryError(string message);

		/// <summary>Clears the dictionary name input, language combo, list selection, and word count label.</summary>
		void ClearMyWordsDictionaryInput();
	}
}

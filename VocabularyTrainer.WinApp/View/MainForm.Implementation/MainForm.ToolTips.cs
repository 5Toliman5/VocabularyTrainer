namespace VocabularyTrainer.WinApp
{
	public partial class MainForm
	{
		// Field initializer, not the constructor: translation rows created during construction already use it.
		private readonly ToolTip _toolTip = new()
		{
			AutoPopDelay = 15000,
			InitialDelay = 500,
			ReshowDelay = 200,
			ShowAlways = true,
		};

		private void InitializeToolTips()
		{
			components.Add(_toolTip);

			// Train yourself
			_toolTip.SetToolTip(DictionaryTrainingComboBox, "Which dictionary to train. \"All dictionaries\" mixes words from every dictionary.");
			_toolTip.SetToolTip(DisplayWordTextBox, "The word to recall. Try to remember its translation before revealing it.");
			_toolTip.SetToolTip(DisplayTranslationTextBox, "The translation appears here after you click \"Translation\".");
			_toolTip.SetToolTip(ShowTranslationButton, "Reveal the translation. The word counts as forgotten and will be shown more often.");
			_toolTip.SetToolTip(ShowNextButton, "Without revealing: you remembered the word, it will be shown less often. After revealing: go to the next word.");
			_toolTip.SetToolTip(DeleteWordButton, "Delete the current word from its dictionary for good.");

			// Add new words
			_toolTip.SetToolTip(DictionaryAddingComboBox, "Dictionary the new word will be added to.");
			_toolTip.SetToolTip(InputWordTextBox, "The word or phrase you want to learn, in the dictionary's language.");
			_toolTip.SetToolTip(TranslationsPanel, "Translations and hints. The first \"Translation\" row is the one shown during training.");
			_toolTip.SetToolTip(AddMoreInfoButton, "Add another row: one more translation, an explanation or a mnemonic.");
			_toolTip.SetToolTip(PinTranslationsButton, "Keep the extra rows after adding a word, so the next word starts with the same row layout.");
			_toolTip.SetToolTip(AddDataButton, "Save the word with its translations to the selected dictionary.");

			// My dictionaries
			_toolTip.SetToolTip(DictionariesListBox, "Your dictionaries. Select one to edit or delete it.");
			_toolTip.SetToolTip(DictionaryNameInputTextBox, "Name of the selected dictionary; must be unique among your dictionaries.");
			_toolTip.SetToolTip(LanguageComboBox, "Language of the words in this dictionary.");
			_toolTip.SetToolTip(MyDictionariesAlgorithmComboBox, "How words are picked for training. Switching keeps the progress of the other algorithm.");
			_toolTip.SetToolTip(AddDictionaryButton, "Create a new dictionary.");
			_toolTip.SetToolTip(UpdateDictionaryButton, "Save name, language and algorithm of the selected dictionary.");
			_toolTip.SetToolTip(DeleteDictionaryButton, "Delete the selected dictionary together with all its words and their training progress. Your last dictionary cannot be deleted.");

			// My words
			_toolTip.SetToolTip(MyWordsDictionaryCombo, "Show words from this dictionary only.");
			_toolTip.SetToolTip(MyWordsLanguageCombo, "Show words in this language only.");
			_toolTip.SetToolTip(MyWordsSearchBox, "Search in words and translations.");
			_toolTip.SetToolTip(MyWordsDateFromPicker, "Show words added on or after this date.");
			_toolTip.SetToolTip(MyWordsDateToPicker, "Show words added on or before this date.");
			_toolTip.SetToolTip(MyWordsApplyButton, "Apply the filters above.");
			_toolTip.SetToolTip(MyWordsResetButton, "Clear all filters and sorting.");
			_toolTip.SetToolTip(MyWordsGrid, "Click a column header to sort by it; click again to reverse the order.");
			_toolTip.SetToolTip(MyWordsPrevButton, "Previous page.");
			_toolTip.SetToolTip(MyWordsNextButton, "Next page.");
			_toolTip.SetToolTip(MyWordsDeleteButton, "Delete the selected word for good.");
		}
	}
}

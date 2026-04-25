using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.Infrastructure.Validation;
using TextBox = System.Windows.Forms.TextBox;

namespace VocabularyTrainer.WinApp
{
	public partial class MainForm
	{
		public string InputWord => InputWordTextBox.Text;
		public string InputTranslation => InputTranslationTextBox.Text;

		public int? SelectedAddingDictionaryId =>
			DictionaryAddingComboBox.SelectedItem is DictionaryComboItem item ? item.Id : null;

		public bool ValidateAddWordInput()
		{
			TextBox[] inputTextBoxes = [InputWordTextBox, InputTranslationTextBox];

			foreach (var textBox in inputTextBoxes)
			{
				if (string.IsNullOrEmpty(textBox.Text))
				{
					AddWordsErrorProvider.SetError(textBox, string.Format(Constants.EmptyInput, textBox.Tag));
					return false;
				}
			}

			return true;
		}

		public void ClearAddWordInput()
		{
			InputWordTextBox.Text = string.Empty;
			InputTranslationTextBox.Text = string.Empty;
			AddWordsErrorProvider.Clear();
		}

		public void ShowAddingDictionaryError(string message) =>
			AddWordsErrorProvider.SetError(DictionaryAddingComboBox, message);

		private void PopulateAddingComboBox(IReadOnlyList<DictionaryDto> dicts)
		{
			var previousId = SelectedAddingDictionaryId;

			DictionaryAddingComboBox.Items.Clear();
			foreach (var d in dicts)
				DictionaryAddingComboBox.Items.Add(new DictionaryComboItem(d.Id, d.ToString()));

			if (DictionaryAddingComboBox.Items.Count == 0) return;

			var targetIndex = 0;
			if (previousId.HasValue)
			{
				for (int i = 0; i < DictionaryAddingComboBox.Items.Count; i++)
				{
					if (DictionaryAddingComboBox.Items[i] is DictionaryComboItem item && item.Id == previousId)
					{
						targetIndex = i;
						break;
					}
				}
			}
			DictionaryAddingComboBox.SelectedIndex = targetIndex;
			AddWordsErrorProvider.SetError(DictionaryAddingComboBox, string.Empty);
		}

		private void AddNewWord(object sender, EventArgs e) => AddWordRequested?.Invoke(sender, e);

		private void ValidateTextBox(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			if (!MainFormValidator.ValidateTextBoxInput(textBox))
			{
				AddWordsErrorProvider.SetError(textBox, "Input must contain only letters, apostrophes, hyphens, and spaces.");
				textBox.Text = string.Empty;
			}
		}

		private void TextBox_SwitchFocus(object? sender, KeyEventArgs e)
		{
			if (e.KeyCode is Keys.Up or Keys.Down)
			{
				if (sender == InputWordTextBox) InputTranslationTextBox.Focus();
				else if (sender == InputTranslationTextBox) InputWordTextBox.Focus();
				else if (sender == DisplayWordTextBox) DisplayTranslationTextBox.Focus();
				else if (sender == DisplayTranslationTextBox) DisplayWordTextBox.Focus();

				e.Handled = true;
			}
		}
	}
}

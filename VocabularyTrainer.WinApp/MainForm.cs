using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.Infrastructure.Validation;
using VocabularyTrainer.WinApp.View;
using TextBox = System.Windows.Forms.TextBox;

namespace WinApp
{
	public partial class MainForm : Form, IMainFormView
	{
		public MainForm()
		{
			InitializeComponent();
		}

		public event EventHandler<string>? UserChanged;
		public event EventHandler? AddWordRequested;
		public event EventHandler? ShowNextWordRequested;
		public event EventHandler? ShowTranslationRequested;
		public event EventHandler? DeleteWordRequested;

		public string InputWord => InputWordTextBox.Text;

		public string InputTranslation => InputTranslationTextBox.Text;

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

		public void ClearShowWordOutput()
		{
			DisplayWordTextBox.Text = string.Empty;
			DisplayTranslationTextBox.Text = string.Empty;
		}

		public void ShowError(string message) => MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

		public void DisplayNewWord(string word) => DisplayWordTextBox.Text = word;

		public void DisplayTranslation(string translation) => DisplayTranslationTextBox.Text = translation;

		public void SetShowNextButtonText(string text) => ShowNextButton.Text = text;

		private void ValidateTextBox(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			if (!MainFormValidator.ValidateTextBoxInput(textBox))
			{
				AddWordsErrorProvider.SetError(textBox, "Input must contain only letters and digits.");
				textBox.Text = string.Empty;
			}
		}

		private void TrainYourSelfPageEnter(object sender, EventArgs e)
		{
			var userName = CurrentUserTextBox.Text;
			if (!string.IsNullOrEmpty(userName))
			{
				UserChanged?.Invoke(this, userName);
			}
			else
			{
				// TODO: request the user to log in
			}
		}

		private void AddNewWord(object sender, EventArgs e) => AddWordRequested?.Invoke(sender, e);

		private void ShowNextWord(object sender, EventArgs e) => ShowNextWordRequested?.Invoke(sender, e);

		private void ShowTranslation(object sender, EventArgs e) => ShowTranslationRequested?.Invoke(sender, e);

		private void DeleteWord(object sender, EventArgs e) => DeleteWordRequested?.Invoke(sender, e);

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
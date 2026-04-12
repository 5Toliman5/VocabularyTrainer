using System.Globalization;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.Infrastructure.Validation;
using VocabularyTrainer.WinApp.View;
using TextBox = System.Windows.Forms.TextBox;

namespace WinApp
{
	/// <summary>Main application form; implements <see cref="IMainFormView"/> and wires UI controls to view events.</summary>
	public partial class MainForm : Form, IMainFormView
	{
		private readonly CultureInfo[] _neutralCultures;
		private List<DictionaryDto> _dictionaryList = [];
		private bool _suppressDictionaryEvents = false;

		/// <summary>Initializes the form, populates the language combo box, and sets up control defaults.</summary>
		public MainForm()
		{
			InitializeComponent();
			_neutralCultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
				.Where(c => !string.IsNullOrEmpty(c.Name))
				.OrderBy(c => c.EnglishName)
				.ToArray();
			InitializeLanguageComboBox();
		}

		// --- Events ---
		/// <inheritdoc/>
		public event EventHandler<string>? UserChanged;
		/// <inheritdoc/>
		public event EventHandler<int?>? TrainingDictionaryChanged;
		/// <inheritdoc/>
		public event EventHandler? AddWordRequested;
		/// <inheritdoc/>
		public event EventHandler? ShowNextWordRequested;
		/// <inheritdoc/>
		public event EventHandler? ShowTranslationRequested;
		/// <inheritdoc/>
		public event EventHandler? DeleteWordRequested;
		/// <inheritdoc/>
		public event EventHandler? AddDictionaryRequested;
		/// <inheritdoc/>
		public event EventHandler? UpdateDictionaryRequested;
		/// <inheritdoc/>
		public event EventHandler? DeleteDictionaryRequested;
		/// <inheritdoc/>
		public event EventHandler? MyWordsPageEntered;

		// --- Properties ---
		/// <inheritdoc/>
		public string CurrentUserName => CurrentUserTextBox.Text;

		/// <inheritdoc/>
		public string InputWord => InputWordTextBox.Text;

		/// <inheritdoc/>
		public string InputTranslation => InputTranslationTextBox.Text;

		/// <inheritdoc/>
		public int? SelectedAddingDictionaryId =>
			DictionaryAddingComboBox.SelectedItem is DictionaryComboItem item ? item.Id : null;

		/// <inheritdoc/>
		public int? SelectedMyWordsDictionaryId =>
			DictionariesListBox.SelectedItem is DictionaryDto dto ? dto.Id : null;

		/// <inheritdoc/>
		public string InputDictionaryName => DictionaryNameInputTextBox.Text;

		/// <inheritdoc/>
		public string? InputLanguageCode
		{
			get
			{
				var text = LanguageComboBox.Text;
				if (string.IsNullOrWhiteSpace(text)) return null;
				var culture = Array.Find(_neutralCultures, c =>
					c.EnglishName.Equals(text, StringComparison.OrdinalIgnoreCase));
				return culture?.Name;
			}
		}

		// --- Word display ---
		/// <inheritdoc/>
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

		/// <inheritdoc/>
		public void ClearAddWordInput()
		{
			InputWordTextBox.Text = string.Empty;
			InputTranslationTextBox.Text = string.Empty;
			AddWordsErrorProvider.Clear();
		}

		/// <inheritdoc/>
		public void ClearShowWordOutput()
		{
			DisplayWordTextBox.Text = string.Empty;
			DisplayTranslationTextBox.Text = string.Empty;
			DictionaryOfWordLabel.Text = string.Empty;
		}

		/// <inheritdoc/>
		public void ShowError(string message) => MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

		/// <inheritdoc/>
		public void DisplayNewWord(string word) => DisplayWordTextBox.Text = word;

		/// <inheritdoc/>
		public void DisplayTranslation(string translation) => DisplayTranslationTextBox.Text = translation;

		/// <inheritdoc/>
		public void SetCurrentWordDictionary(string dictName) => DictionaryOfWordLabel.Text = dictName;

		/// <inheritdoc/>
		public void SetShowNextButtonText(string text) => ShowNextButton.Text = text;

		// --- Dictionary management ---
		/// <inheritdoc/>
		public void LoadDictionaries(IReadOnlyList<DictionaryDto> dicts)
		{
			_dictionaryList = dicts.ToList();

			_suppressDictionaryEvents = true;
			try
			{
				PopulateTrainingComboBox(dicts);
				PopulateAddingComboBox(dicts);
				PopulateMyWordsDictionaries(dicts);
			}
			finally
			{
				_suppressDictionaryEvents = false;
			}
		}

		/// <inheritdoc/>
		public void ShowAddingDictionaryError(string message)
		{
			AddWordsErrorProvider.SetError(DictionaryAddingComboBox, message);
		}

		/// <inheritdoc/>
		public void ClearMyWordsDictionaryInput()
		{
			DictionaryNameInputTextBox.Text = string.Empty;
			LanguageComboBox.Text = string.Empty;
			LanguageComboBox.SelectedIndex = -1;
			DictionariesListBox.SelectedIndex = -1;
			WordCountLabel.Text = string.Empty;
		}

		// --- Private UI helpers ---
		private void InitializeLanguageComboBox()
		{
			foreach (var culture in _neutralCultures)
				LanguageComboBox.Items.Add(culture.EnglishName);

			LanguageComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			LanguageComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
		}

		private void PopulateTrainingComboBox(IReadOnlyList<DictionaryDto> dicts)
		{
			var previousId = SelectedTrainingDictionaryId;

			DictionaryTrainingComboBox.Items.Clear();
			DictionaryTrainingComboBox.Items.Add(new DictionaryComboItem(null, "All"));
			foreach (var d in dicts)
				DictionaryTrainingComboBox.Items.Add(new DictionaryComboItem(d.Id, d.ToString()));

			var targetIndex = 0;
			if (previousId.HasValue)
			{
				for (int i = 1; i < DictionaryTrainingComboBox.Items.Count; i++)
				{
					if (DictionaryTrainingComboBox.Items[i] is DictionaryComboItem item && item.Id == previousId)
					{
						targetIndex = i;
						break;
					}
				}
			}
			DictionaryTrainingComboBox.SelectedIndex = targetIndex;
		}

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

		private void PopulateMyWordsDictionaries(IReadOnlyList<DictionaryDto> dicts)
		{
			var previousId = SelectedMyWordsDictionaryId;

			DictionariesListBox.Items.Clear();
			foreach (var d in dicts)
				DictionariesListBox.Items.Add(d);

			if (previousId.HasValue)
			{
				for (int i = 0; i < DictionariesListBox.Items.Count; i++)
				{
					if (((DictionaryDto)DictionariesListBox.Items[i]).Id == previousId)
					{
						DictionariesListBox.SelectedIndex = i;
						return;
					}
				}
			}
		}

		private int? SelectedTrainingDictionaryId =>
			DictionaryTrainingComboBox.SelectedItem is DictionaryComboItem item ? item.Id : null;

		// --- Event handlers ---
		private void TrainYourSelfPageEnter(object sender, EventArgs e)
		{
			var userName = CurrentUserTextBox.Text;
			if (!string.IsNullOrEmpty(userName))
				UserChanged?.Invoke(this, userName);
			else
			{
				// TODO: request the user to log in
			}
		}

		private void MyWordsPage_Enter(object sender, EventArgs e) =>
			MyWordsPageEntered?.Invoke(this, EventArgs.Empty);

		private void DictionaryTrainingComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_suppressDictionaryEvents) return;
			TrainingDictionaryChanged?.Invoke(this, SelectedTrainingDictionaryId);
		}

		private void DictionariesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (DictionariesListBox.SelectedItem is not DictionaryDto selected)
			{
				WordCountLabel.Text = string.Empty;
				return;
			}

			DictionaryNameInputTextBox.Text = selected.Name;
			WordCountLabel.Text = $"{selected.WordCount} word{(selected.WordCount == 1 ? "" : "s")}";

			if (!string.IsNullOrEmpty(selected.LanguageCode))
			{
				var culture = Array.Find(_neutralCultures, c => c.Name == selected.LanguageCode);
				LanguageComboBox.Text = culture?.EnglishName ?? string.Empty;
			}
			else
			{
				LanguageComboBox.Text = string.Empty;
			}
		}

		private void AddNewWord(object sender, EventArgs e) => AddWordRequested?.Invoke(sender, e);

		private void ShowNextWord(object sender, EventArgs e) => ShowNextWordRequested?.Invoke(sender, e);

		private void ShowTranslation(object sender, EventArgs e) => ShowTranslationRequested?.Invoke(sender, e);

		private void DeleteWord(object sender, EventArgs e) => DeleteWordRequested?.Invoke(sender, e);

		private void AddDictionary(object sender, EventArgs e) => AddDictionaryRequested?.Invoke(sender, e);

		private void UpdateDictionary(object sender, EventArgs e) => UpdateDictionaryRequested?.Invoke(sender, e);

		private void DeleteDictionary(object sender, EventArgs e) => DeleteDictionaryRequested?.Invoke(sender, e);

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

		private sealed class DictionaryComboItem(int? id, string display)
		{
			public int? Id => id;
			public override string ToString() => display;
		}
	}
}

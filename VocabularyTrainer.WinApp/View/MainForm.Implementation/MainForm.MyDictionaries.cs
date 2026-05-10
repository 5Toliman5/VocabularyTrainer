using System.Globalization;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp
{
	public partial class MainForm
	{
		public int? SelectedMyWordsDictionaryId
		{
			get
			{
				return DictionariesListBox.SelectedItem is DictionaryDto dto
					? dto.Id
					: null;
			}
		}

		public string InputDictionaryName => DictionaryNameInputTextBox.Text;

		public string SelectedDictionaryAlgorithmCode
		{
			get
			{
				return MyDictionariesAlgorithmComboBox.SelectedItem is AlgorithmComboItem item
					? item.Code
					: null;
			}
		}

		public string InputLanguageCode
		{
			get
			{
				var text = LanguageComboBox.Text;

				if (string.IsNullOrWhiteSpace(text))
				{
					return null;
				}

				CultureInfo matchedCulture = null;

				foreach (var culture in _neutralCultures)
				{
					if (culture.EnglishName.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						matchedCulture = culture;
						break;
					}
				}

				return matchedCulture?.Name;
			}
		}

		public void ClearMyWordsDictionaryInput()
		{
			DictionaryNameInputTextBox.Text = string.Empty;
			LanguageComboBox.Text = string.Empty;
			LanguageComboBox.SelectedIndex = -1;
			MyDictionariesAlgorithmComboBox.SelectedIndex = -1;
			DictionariesListBox.SelectedIndex = -1;
			WordCountLabel.Text = string.Empty;
			UpdateDictionaryButton.Enabled = false;
			DeleteDictionaryButton.Enabled = false;
		}

		public void LoadAlgorithms(IReadOnlyList<AlgorithmInfo> algorithms)
		{
			var previousCode = SelectedDictionaryAlgorithmCode;

			MyDictionariesAlgorithmComboBox.Items.Clear();

			foreach (var algorithm in algorithms)
			{
				MyDictionariesAlgorithmComboBox.Items.Add(new AlgorithmComboItem(algorithm.Code, algorithm.DisplayName));
			}

			if (previousCode is not null)
			{
				SelectAlgorithmByCode(previousCode);
			}
		}

		private void SelectAlgorithmByCode(string code)
		{
			for (int i = 0; i < MyDictionariesAlgorithmComboBox.Items.Count; i++)
			{
				if (MyDictionariesAlgorithmComboBox.Items[i] is AlgorithmComboItem item
					&& string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase))
				{
					MyDictionariesAlgorithmComboBox.SelectedIndex = i;
					return;
				}
			}

			MyDictionariesAlgorithmComboBox.SelectedIndex = -1;
		}

		private void InitializeLanguageComboBox()
		{
			foreach (var culture in _neutralCultures)
			{
				LanguageComboBox.Items.Add(culture.EnglishName);
			}

			LanguageComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			LanguageComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
		}

		private void PopulateMyDictionariesListBox(IReadOnlyList<DictionaryDto> dictionaries)
		{
			var previousId = SelectedMyWordsDictionaryId;

			DictionariesListBox.Items.Clear();

			foreach (var dictionary in dictionaries)
			{
				DictionariesListBox.Items.Add(dictionary);
			}

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

		private void MyDictionariesPage_Enter(object sender, EventArgs e)
		{
			MyDictionariesPageEntered?.Invoke(this, EventArgs.Empty);
		}

		private void DictionariesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (DictionariesListBox.SelectedItem is not DictionaryDto selected)
			{
				WordCountLabel.Text = string.Empty;
				UpdateDictionaryButton.Enabled = false;
				DeleteDictionaryButton.Enabled = false;
				return;
			}

			DictionaryNameInputTextBox.Text = selected.Name;

			var pluralSuffix = selected.WordCount == 1
				? ""
				: "s";

			WordCountLabel.Text = $"{selected.WordCount} word{pluralSuffix}";
			UpdateDictionaryButton.Enabled = true;
			DeleteDictionaryButton.Enabled = true;

			if (!string.IsNullOrEmpty(selected.LanguageCode))
			{
				var culture = Array.Find(_neutralCultures, c => c.Name == selected.LanguageCode);
				LanguageComboBox.Text = culture?.EnglishName ?? string.Empty;
			}
			else
			{
				LanguageComboBox.Text = string.Empty;
			}

			SelectAlgorithmByCode(selected.AlgorithmCode);
		}

		private void AddDictionary(object sender, EventArgs e)
		{
			AddDictionaryClickRequested?.Invoke(this, EventArgs.Empty);
		}

		private void UpdateDictionary(object sender, EventArgs e)
		{
			UpdateDictionaryRequested?.Invoke(sender, e);
		}

		private void DeleteDictionary(object sender, EventArgs e)
		{
			DeleteDictionaryRequested?.Invoke(sender, e);
		}
	}
}

using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp
{
	public partial class MainForm
	{
		public string CurrentUserName => CurrentUserTextBox.Text;

		public void ClearShowWordOutput()
		{
			DisplayWordTextBox.Text = string.Empty;
			DisplayTranslationTextBox.Text = string.Empty;
			DictionaryOfWordLabel.Text = string.Empty;
		}

		public void DisplayNewWord(string word) => DisplayWordTextBox.Text = word;
		public void DisplayTranslation(string translation) => DisplayTranslationTextBox.Text = translation;
		public void SetCurrentWordDictionary(string dictName) => DictionaryOfWordLabel.Text = dictName;
		public void SetShowNextButtonText(string text) => ShowNextButton.Text = text;

		private int? SelectedTrainingDictionaryId =>
			DictionaryTrainingComboBox.SelectedItem is DictionaryComboItem item ? item.Id : null;

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

		private void TrainYourSelfPageEnter(object sender, EventArgs e)
		{
			var userName = CurrentUserTextBox.Text;
			if (!string.IsNullOrEmpty(userName))
				UserChanged?.Invoke(this, userName);
		}

		private void DictionaryTrainingComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_suppressDictionaryEvents) return;
			TrainingDictionaryChanged?.Invoke(this, SelectedTrainingDictionaryId);
		}

		private void ShowNextWord(object sender, EventArgs e) => ShowNextWordRequested?.Invoke(sender, e);
		private void ShowTranslation(object sender, EventArgs e) => ShowTranslationRequested?.Invoke(sender, e);
		private void DeleteWord(object sender, EventArgs e) => DeleteWordRequested?.Invoke(sender, e);
	}
}

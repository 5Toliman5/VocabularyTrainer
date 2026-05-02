using System.Globalization;

namespace VocabularyTrainer.WinApp.Forms
{
	internal sealed class AddDictionaryForm : Form
	{
		private readonly TextBox _nameTextBox;
		private readonly ComboBox _languageComboBox;
		private readonly CultureInfo[] _neutralCultures;

		public string DictionaryName => _nameTextBox.Text.Trim();

		public string? DictionaryLanguageCode
		{
			get
			{
				var text = _languageComboBox.Text;
				if (string.IsNullOrWhiteSpace(text)) return null;
				var culture = Array.Find(_neutralCultures, c =>
					c.EnglishName.Equals(text, StringComparison.OrdinalIgnoreCase));
				return culture?.Name;
			}
		}

		public AddDictionaryForm(CultureInfo[] neutralCultures)
		{
			_neutralCultures = neutralCultures;

			Text = "Add Dictionary";
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			ClientSize = new Size(400, 160);

			var nameLabel = new Label
			{
				Text = "Name",
				Location = new Point(16, 20),
				AutoSize = true,
				ForeColor = Color.Black
			};

			_nameTextBox = new TextBox
			{
				Location = new Point(110, 17),
				Size = new Size(270, 24),
				MaxLength = 50
			};

			var languageLabel = new Label
			{
				Text = "Language",
				Location = new Point(16, 60),
				AutoSize = true,
				ForeColor = Color.Black
			};

			_languageComboBox = new ComboBox
			{
				Location = new Point(110, 57),
				Size = new Size(270, 24)
			};
			foreach (var culture in neutralCultures)
				_languageComboBox.Items.Add(culture.EnglishName);
			_languageComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			_languageComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

			var addButton = new Button
			{
				Text = "Add",
				Size = new Size(100, 34),
				Location = new Point(90, 108)
			};
			addButton.Click += (_, _) =>
			{
				if (string.IsNullOrWhiteSpace(DictionaryName))
				{
					MessageBox.Show("Dictionary name cannot be empty.", "Validation",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				DialogResult = DialogResult.OK;
				Close();
			};

			var cancelButton = new Button
			{
				Text = "Cancel",
				Size = new Size(100, 34),
				Location = new Point(210, 108),
				DialogResult = DialogResult.Cancel
			};

			Controls.AddRange([nameLabel, _nameTextBox, languageLabel, _languageComboBox, addButton, cancelButton]);
			AcceptButton = addButton;
			CancelButton = cancelButton;
		}
	}
}

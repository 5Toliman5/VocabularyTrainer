using System.Globalization;
using System.Linq;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp.View
{
	internal sealed class AddDictionaryForm : Form, IAddDictionaryFormView
	{
		private readonly TextBox _nameTextBox;
		private readonly ComboBox _languageComboBox;
		private readonly ComboBox _algorithmComboBox;
		private CultureInfo[] _neutralCultures = [];

		public event EventHandler? ConfirmRequested;

		public string DictionaryName => _nameTextBox.Text.Trim();

		public string DictionaryLanguageCode
		{
			get
			{
				var text = _languageComboBox.Text;

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

		public string DictionaryAlgorithmCode
		{
			get
			{
				return _algorithmComboBox.SelectedItem is AlgorithmComboItem item
					? item.Code
					: AlgorithmCodes.Default;
			}
		}

		public AddDictionaryForm()
		{
			Text = Constants.AddDictionaryDialogTitle;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			ClientSize = new Size(420, 220);

			var nameLabel = new Label
			{
				Text = "Name",
				Location = new Point(16, 20),
				AutoSize = true,
				ForeColor = Color.Black
			};

			_nameTextBox = new TextBox
			{
				Location = new Point(120, 17),
				Size = new Size(280, 24),
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
				Location = new Point(120, 57),
				Size = new Size(280, 24),
				AutoCompleteMode = AutoCompleteMode.SuggestAppend,
				AutoCompleteSource = AutoCompleteSource.ListItems
			};

			var algorithmLabel = new Label
			{
				Text = "Algorithm",
				Location = new Point(16, 100),
				AutoSize = true,
				ForeColor = Color.Black
			};

			_algorithmComboBox = new ComboBox
			{
				Location = new Point(120, 97),
				Size = new Size(280, 24),
				DropDownStyle = ComboBoxStyle.DropDownList
			};

			var addButton = new Button
			{
				Text = "Add",
				Size = new Size(100, 34),
				Location = new Point(100, 165)
			};
			addButton.Click += (_, _) => ConfirmRequested?.Invoke(this, EventArgs.Empty);

			var cancelButton = new Button
			{
				Text = "Cancel",
				Size = new Size(100, 34),
				Location = new Point(220, 165),
				DialogResult = DialogResult.Cancel
			};

			Controls.AddRange(new Control[] {
				nameLabel, _nameTextBox,
				languageLabel, _languageComboBox,
				algorithmLabel, _algorithmComboBox,
				addButton, cancelButton
			});
			AcceptButton = addButton;
			CancelButton = cancelButton;
		}

		public void Initialize(IReadOnlyList<CultureInfo> neutralCultures, IReadOnlyList<AlgorithmInfo> algorithms)
		{
			_neutralCultures = neutralCultures is CultureInfo[] existingArray
				? existingArray
				: neutralCultures.ToArray();

			_languageComboBox.Items.Clear();

			foreach (var culture in _neutralCultures)
			{
				_languageComboBox.Items.Add(culture.EnglishName);
			}

			_algorithmComboBox.Items.Clear();

			if (algorithms is { Count: > 0 })
			{
				foreach (var algorithm in algorithms)
				{
					_algorithmComboBox.Items.Add(new AlgorithmComboItem(algorithm.Code, algorithm.DisplayName));
				}

				_algorithmComboBox.SelectedIndex = FindDefaultAlgorithmIndex();
			}
			else
			{
				_algorithmComboBox.Items.Add(new AlgorithmComboItem(AlgorithmCodes.Default, "Weight-based"));
				_algorithmComboBox.SelectedIndex = 0;
			}
		}

		private int FindDefaultAlgorithmIndex()
		{
			for (int i = 0; i < _algorithmComboBox.Items.Count; i++)
			{
				if (_algorithmComboBox.Items[i] is AlgorithmComboItem item
					&& string.Equals(item.Code, AlgorithmCodes.Default, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}

			return 0;
		}

		public void ShowValidationError(string message)
		{
			MessageBox.Show(this, message, Constants.ValidationCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		public void CompleteSuccessfully()
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

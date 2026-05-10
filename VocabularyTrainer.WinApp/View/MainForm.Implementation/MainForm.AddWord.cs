using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.Infrastructure.Validation;
using TextBox = System.Windows.Forms.TextBox;

namespace VocabularyTrainer.WinApp
{
	public partial class MainForm
	{
		private static readonly TranslationKind[] AllTranslationKinds =
		[
			TranslationKind.Translation,
			TranslationKind.Explanation,
			TranslationKind.Mnemonic,
		];

		private const int MaxTranslationCount = 3;
		private const int MaxExplanationCount = 1;
		private const int MaxMnemonicCount = 1;

		// Suppresses cascade refreshes while we re-populate combo items in code,
		// otherwise SelectedIndexChanged on one row would fire RefreshKindCombos
		// which mutates the others mid-iteration.
		private bool _suppressTranslationRowEvents;

		public string InputWord => InputWordTextBox.Text;

		public int? SelectedAddingDictionaryId
		{
			get
			{
				return DictionaryAddingComboBox.SelectedItem is DictionaryComboItem item
					? item.Id
					: null;
			}
		}

		public IReadOnlyList<WordTranslationDto> InputTranslations
		{
			get
			{
				var rows = GetTranslationRows();
				var result = new List<WordTranslationDto>(rows.Count);

				foreach (var row in rows)
				{
					var text = row.TextBox.Text?.Trim() ?? string.Empty;

					if (text.Length == 0)
					{
						continue;
					}

					result.Add(new WordTranslationDto(text, row.Kind));
				}

				return result;
			}
		}

		public bool ValidateAddWordInput()
		{
			if (string.IsNullOrEmpty(InputWordTextBox.Text))
			{
				AddWordsErrorProvider.SetError(InputWordTextBox, string.Format(Constants.EmptyInput, InputWordTextBox.Tag));
				return false;
			}

			var rows = GetTranslationRows();
			var primaryRow = rows.FirstOrDefault(r => r.Kind == TranslationKind.Translation);

			if (primaryRow is null || string.IsNullOrWhiteSpace(primaryRow.TextBox.Text))
			{
				var target = primaryRow?.TextBox ?? rows.FirstOrDefault()?.TextBox;

				if (target is not null)
				{
					AddWordsErrorProvider.SetError(target, Constants.TranslationRequired);
				}

				return false;
			}

			return true;
		}

		public void ClearAddWordInput()
		{
			InputWordTextBox.Text = string.Empty;
			AddWordsErrorProvider.Clear();

			if (PinTranslationsButton.Checked)
			{
				ClearTranslationRowTexts();
			}
			else
			{
				ResetTranslationRows();
			}
		}

		private void PinTranslationsButton_CheckedChanged(object? sender, EventArgs e)
		{
			PinTranslationsButton.Text = PinTranslationsButton.Checked ? "Pinned" : "Pin";
		}

		private void RefreshPinButton()
		{
			var hasExtraRows = GetTranslationRows().Count > 1;

			PinTranslationsButton.Enabled = hasExtraRows;

			if (!hasExtraRows && PinTranslationsButton.Checked)
			{
				PinTranslationsButton.Checked = false;
			}
		}

		private void ClearTranslationRowTexts()
		{
			foreach (var row in GetTranslationRows())
			{
				row.TextBox.Text = string.Empty;
			}
		}

		public void ShowAddingDictionaryError(string message)
		{
			AddWordsErrorProvider.SetError(DictionaryAddingComboBox, message);
		}

		private void InitializeTranslationsEditor()
		{
			TranslationsPanel.Controls.Clear();
			AddTranslationRow(TranslationKind.Translation);
			RefreshAddMoreInfoButton();
		}

		private void ResetTranslationRows()
		{
			TranslationsPanel.SuspendLayout();
			TranslationsPanel.Controls.Clear();
			TranslationsPanel.ResumeLayout();

			AddTranslationRow(TranslationKind.Translation);
			RefreshAddMoreInfoButton();
		}

		private void AddTranslationRow(TranslationKind kind)
		{
			var row = new Panel
			{
				Margin = new Padding(0, 0, 0, 6),
				Size = new Size(650, 40),
			};

			var textBox = new TextBox
			{
				Location = new Point(0, 4),
				Size = new Size(420, 30),
				MaxLength = 500,
			};

			var kindCombo = new ComboBox
			{
				Location = new Point(430, 4),
				Size = new Size(180, 30),
				DropDownStyle = ComboBoxStyle.DropDownList,
			};
			kindCombo.SelectedIndexChanged += KindCombo_SelectedIndexChanged;

			var removeButton = new Button
			{
				Location = new Point(617, 3),
				Size = new Size(34, 32),
				Text = "−",
				ForeColor = Color.Black,
				UseVisualStyleBackColor = true,
				TabStop = false,
			};
			removeButton.Click += RemoveTranslationRow_Click;

			row.Tag = new TranslationRowTag(textBox, kindCombo, removeButton, kind);
			row.Controls.Add(textBox);
			row.Controls.Add(kindCombo);
			row.Controls.Add(removeButton);

			TranslationsPanel.Controls.Add(row);

			RefreshKindCombos();
			RefreshRemoveButtonsVisibility();
			RefreshPinButton();
		}

		private void KindCombo_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (_suppressTranslationRowEvents)
			{
				return;
			}

			if (sender is not ComboBox combo || combo.SelectedItem is not TranslationKind newKind)
			{
				return;
			}

			var row = combo.Parent;

			if (row?.Tag is TranslationRowTag tag)
			{
				row.Tag = tag with { Kind = newKind };
			}

			RefreshKindCombos();
		}

		private void RemoveTranslationRow_Click(object? sender, EventArgs e)
		{
			if (sender is not Button button || button.Parent is not Panel row)
			{
				return;
			}

			TranslationsPanel.Controls.Remove(row);
			row.Dispose();

			RefreshKindCombos();
			RefreshRemoveButtonsVisibility();
			RefreshAddMoreInfoButton();
			RefreshPinButton();
		}

		private void AddMoreInfoButton_Click(object? sender, EventArgs e)
		{
			var nextKind = FindFirstAvailableKind();

			if (nextKind is null)
			{
				return;
			}

			AddTranslationRow(nextKind.Value);
			RefreshAddMoreInfoButton();
		}

		// Each row's combo lists every kind whose per-kind cap (Translation: 3,
		// Explanation/Mnemonic: 1) isn't yet reached, plus the row's own current
		// selection — so a row can always keep the kind it already has.
		private void RefreshKindCombos()
		{
			_suppressTranslationRowEvents = true;
			try
			{
				var rows = GetTranslationRows();
				var counts = CountKinds(rows);

				foreach (var row in rows)
				{
					var available = AllTranslationKinds
						.Where(k => k == row.Kind || counts.GetValueOrDefault(k) < MaxCountFor(k))
						.ToArray();

					var combo = row.Combo;
					var previous = row.Kind;

					combo.BeginUpdate();
					combo.Items.Clear();

					foreach (var kind in available)
					{
						combo.Items.Add(kind);
					}

					combo.SelectedItem = previous;
					combo.EndUpdate();
				}
			}
			finally
			{
				_suppressTranslationRowEvents = false;
			}
		}

		private void RefreshRemoveButtonsVisibility()
		{
			var rows = GetTranslationRows();
			var hideRemove = rows.Count <= 1;

			foreach (var row in rows)
			{
				row.RemoveButton.Visible = !hideRemove;
			}
		}

		private void RefreshAddMoreInfoButton()
		{
			AddMoreInfoButton.Enabled = FindFirstAvailableKind() is not null;
		}

		// Picks the next kind to add when "+ Add a translation" is clicked.
		// Iterates AllTranslationKinds in declaration order so Translation
		// (which has the largest cap) is preferred until full.
		private TranslationKind? FindFirstAvailableKind()
		{
			var counts = CountKinds(GetTranslationRows());

			foreach (var kind in AllTranslationKinds)
			{
				if (counts.GetValueOrDefault(kind) < MaxCountFor(kind))
				{
					return kind;
				}
			}

			return null;
		}

		private static Dictionary<TranslationKind, int> CountKinds(IReadOnlyList<TranslationRowTag> rows)
		{
			var counts = new Dictionary<TranslationKind, int>();

			foreach (var row in rows)
			{
				counts[row.Kind] = counts.GetValueOrDefault(row.Kind) + 1;
			}

			return counts;
		}

		private static int MaxCountFor(TranslationKind kind) => kind switch
		{
			TranslationKind.Translation => MaxTranslationCount,
			TranslationKind.Explanation => MaxExplanationCount,
			TranslationKind.Mnemonic => MaxMnemonicCount,
			_ => 0,
		};

		private List<TranslationRowTag> GetTranslationRows()
		{
			var result = new List<TranslationRowTag>(TranslationsPanel.Controls.Count);

			foreach (Control control in TranslationsPanel.Controls)
			{
				if (control.Tag is TranslationRowTag tag)
				{
					result.Add(tag);
				}
			}

			return result;
		}

		private void PopulateAddingComboBox(IReadOnlyList<DictionaryDto> dictionaries)
		{
			var previousId = SelectedAddingDictionaryId;

			DictionaryAddingComboBox.Items.Clear();

			foreach (var dictionary in dictionaries)
			{
				DictionaryAddingComboBox.Items.Add(new DictionaryComboItem(dictionary.Id, dictionary.ToString()));
			}

			if (DictionaryAddingComboBox.Items.Count == 0)
			{
				return;
			}

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

		private void AddNewWord(object sender, EventArgs e)
		{
			AddWordRequested?.Invoke(sender, e);
		}

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
				if (sender == DisplayWordTextBox)
				{
					DisplayTranslationTextBox.Focus();
				}
				else if (sender == DisplayTranslationTextBox)
				{
					DisplayWordTextBox.Focus();
				}

				e.Handled = true;
			}
		}

		// Holds the controls of one translation row plus its current Kind.
		private sealed record TranslationRowTag(
			TextBox TextBox,
			ComboBox Combo,
			Button RemoveButton,
			TranslationKind Kind);
	}
}

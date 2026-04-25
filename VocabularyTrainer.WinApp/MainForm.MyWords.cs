using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp
{
	public partial class MainForm
	{
		private IReadOnlyList<WordDto> _currentMyWordsWords = [];
		private string _myWordsSortColumn = "DateAdded";
		private bool _myWordsSortDesc = true;

		public int? MyWordsDictionaryId =>
			MyWordsDictionaryCombo.SelectedItem is DictionaryComboItem item && item.Id.HasValue ? item.Id : null;

		public string? MyWordsLanguage =>
			MyWordsLanguageCombo.SelectedItem is string code && code != string.Empty ? code : null;

		public string? MyWordsSearch =>
			string.IsNullOrWhiteSpace(MyWordsSearchBox.Text) ? null : MyWordsSearchBox.Text.Trim();

		public DateTime? MyWordsDateFrom =>
			MyWordsDateFromPicker.Checked ? MyWordsDateFromPicker.Value.Date : null;

		public DateTime? MyWordsDateTo =>
			MyWordsDateToPicker.Checked ? MyWordsDateToPicker.Value.Date.AddDays(1).AddTicks(-1) : null;

		public WordDto? SelectedMyWordsWord
		{
			get
			{
				if (MyWordsGrid.SelectedRows.Count == 0) return null;
				var index = MyWordsGrid.SelectedRows[0].Index;
				if (index < 0 || index >= _currentMyWordsWords.Count) return null;
				return _currentMyWordsWords[index];
			}
		}

		public void LoadMyWordsPage(PagedResult<WordDto> result)
		{
			_currentMyWordsWords = result.Items;
			MyWordsGrid.DataSource = result.Items.ToList();
			MyWordsGrid.ClearSelection();

			UpdateSortGlyph();

			MyWordsPageLabel.Text = $"Page {result.Page} of {result.TotalPages}  ({result.TotalCount} words)";
			MyWordsPrevButton.Enabled = result.HasPreviousPage;
			MyWordsNextButton.Enabled = result.HasNextPage;
		}

		private void InitializeMyWordsGrid()
		{
			MyWordsGrid.AutoGenerateColumns = false;
			MyWordsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			MyWordsGrid.MultiSelect = false;
			MyWordsGrid.ReadOnly = true;
			MyWordsGrid.AllowUserToAddRows = false;
			MyWordsGrid.AllowUserToDeleteRows = false;
			MyWordsGrid.RowHeadersVisible = false;
			MyWordsGrid.Font = new Font("Segoe UI", 9F);
			MyWordsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			MyWordsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			MyWordsGrid.RowTemplate.Height = 26;

			MyWordsGrid.Columns.AddRange(
				new DataGridViewTextBoxColumn { Name = "Value",          DataPropertyName = "Value",          HeaderText = "Word",        Width = 241, SortMode = DataGridViewColumnSortMode.Programmatic },
				new DataGridViewTextBoxColumn { Name = "Translation",    DataPropertyName = "Translation",    HeaderText = "Translation", Width = 252, SortMode = DataGridViewColumnSortMode.Programmatic },
				new DataGridViewTextBoxColumn { Name = "DictionaryName", DataPropertyName = "DictionaryName", HeaderText = "Dictionary",  Width = 140, SortMode = DataGridViewColumnSortMode.Programmatic },
				new DataGridViewTextBoxColumn { Name = "LanguageCode",   DataPropertyName = "LanguageCode",   HeaderText = "Language",    Width = 80,  SortMode = DataGridViewColumnSortMode.Programmatic },
				new DataGridViewTextBoxColumn
				{
					Name = "DateAdded", DataPropertyName = "DateAdded", HeaderText = "Added",
					Width = 110, SortMode = DataGridViewColumnSortMode.Programmatic,
					DefaultCellStyle = { Format = "dd/MM/yy" }
				},
				new DataGridViewTextBoxColumn { Name = "Weight", DataPropertyName = "Weight", HeaderText = "Weight", Width = 70, SortMode = DataGridViewColumnSortMode.Programmatic }
			);

			MyWordsGrid.ColumnHeaderMouseClick += MyWordsGrid_ColumnHeaderMouseClick;
		}

		private void PopulateMyWordsDictionaryFilter(IReadOnlyList<DictionaryDto> dicts)
		{
			var previousId = MyWordsDictionaryId;

			MyWordsDictionaryCombo.Items.Clear();
			MyWordsDictionaryCombo.Items.Add(new DictionaryComboItem(null, "All dictionaries"));
			foreach (var d in dicts)
				MyWordsDictionaryCombo.Items.Add(new DictionaryComboItem(d.Id, d.ToString()));

			var targetIndex = 0;
			if (previousId.HasValue)
			{
				for (int i = 1; i < MyWordsDictionaryCombo.Items.Count; i++)
				{
					if (MyWordsDictionaryCombo.Items[i] is DictionaryComboItem item && item.Id == previousId)
					{
						targetIndex = i;
						break;
					}
				}
			}
			MyWordsDictionaryCombo.SelectedIndex = targetIndex;
		}

		private void PopulateMyWordsLanguageFilter(IReadOnlyList<DictionaryDto> dicts)
		{
			var previousCode = MyWordsLanguage;

			MyWordsLanguageCombo.Items.Clear();
			MyWordsLanguageCombo.Items.Add(string.Empty);
			foreach (var code in dicts.Where(d => !string.IsNullOrEmpty(d.LanguageCode))
			                          .Select(d => d.LanguageCode!)
			                          .Distinct()
			                          .OrderBy(c => c))
				MyWordsLanguageCombo.Items.Add(code);

			var targetIndex = 0;
			if (previousCode != null)
			{
				for (int i = 1; i < MyWordsLanguageCombo.Items.Count; i++)
				{
					if ((string)MyWordsLanguageCombo.Items[i] == previousCode)
					{
						targetIndex = i;
						break;
					}
				}
			}
			MyWordsLanguageCombo.SelectedIndex = targetIndex;
		}

		private void MyWordsPage_Enter(object sender, EventArgs e) =>
			MyWordsPageEntered?.Invoke(this, EventArgs.Empty);

		private void MyWordsApplyButton_Click(object sender, EventArgs e) =>
			ApplyWordFilterRequested?.Invoke(sender, e);

		private void MyWordsPrevButton_Click(object sender, EventArgs e) =>
			PreviousWordPageRequested?.Invoke(sender, e);

		private void MyWordsNextButton_Click(object sender, EventArgs e) =>
			NextWordPageRequested?.Invoke(sender, e);

		private void MyWordsDeleteButton_Click(object sender, EventArgs e)
		{
			if (MyWordsGrid.SelectedRows.Count == 0) return;

			var word = MyWordsGrid.SelectedRows[0].Cells["Value"].Value?.ToString() ?? string.Empty;
			var confirm = MessageBox.Show(
				string.Format(Constants.DeleteWordConfirmation, word),
				"Delete word",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);

			if (confirm == DialogResult.Yes)
				DeleteMyWordsWordRequested?.Invoke(sender, e);
		}

		private void MyWordsGrid_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
		{
			var col = MyWordsGrid.Columns[e.ColumnIndex].Name;
			if (col == _myWordsSortColumn)
				_myWordsSortDesc = !_myWordsSortDesc;
			else
			{
				_myWordsSortColumn = col;
				_myWordsSortDesc = false;
			}
			MyWordsSortChanged?.Invoke(this, col);
		}

		private void UpdateSortGlyph()
		{
			foreach (DataGridViewColumn col in MyWordsGrid.Columns)
				col.HeaderCell.SortGlyphDirection = SortOrder.None;

			if (MyWordsGrid.Columns.Contains(_myWordsSortColumn))
				MyWordsGrid.Columns[_myWordsSortColumn].HeaderCell.SortGlyphDirection =
					_myWordsSortDesc ? SortOrder.Descending : SortOrder.Ascending;
		}
	}
}

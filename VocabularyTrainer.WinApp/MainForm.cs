using System.Globalization;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp
{
	public partial class MainForm : Form, IMainFormView
	{
		internal readonly CultureInfo[] _neutralCultures;
		internal bool _suppressDictionaryEvents;

		public MainForm()
		{
			InitializeComponent();
			_neutralCultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
				.Where(c => !string.IsNullOrEmpty(c.Name))
				.OrderBy(c => c.EnglishName)
				.ToArray();
			InitializeLanguageComboBox();
			InitializeMyWordsGrid();
		}

		// ── Events ────────────────────────────────────────────────────────────
		public event EventHandler<string>? UserChanged;
		public event EventHandler<int?>? TrainingDictionaryChanged;
		public event EventHandler? AddWordRequested;
		public event EventHandler? ShowNextWordRequested;
		public event EventHandler? ShowTranslationRequested;
		public event EventHandler? DeleteWordRequested;
		public event EventHandler? AddDictionaryRequested;
		public event EventHandler? UpdateDictionaryRequested;
		public event EventHandler? DeleteDictionaryRequested;
		public event EventHandler? MyDictionariesPageEntered;
		public event EventHandler? MyWordsPageEntered;
		public event EventHandler? ApplyWordFilterRequested;
		public event EventHandler? PreviousWordPageRequested;
		public event EventHandler? NextWordPageRequested;
		public event EventHandler? DeleteMyWordsWordRequested;
		public event EventHandler<string>? MyWordsSortChanged;
		public event EventHandler? ResetWordFilterRequested;

		// ── Shared view methods ───────────────────────────────────────────────
		public void ShowError(string message) =>
			MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

		public void LoadDictionaries(IReadOnlyList<DictionaryDto> dicts)
		{
			_suppressDictionaryEvents = true;
			try
			{
				PopulateTrainingComboBox(dicts);
				PopulateAddingComboBox(dicts);
				PopulateMyDictionariesListBox(dicts);
				PopulateMyWordsDictionaryFilter(dicts);
				PopulateMyWordsLanguageFilter(dicts);
			}
			finally
			{
				_suppressDictionaryEvents = false;
			}
		}

		// ── Shared helper ─────────────────────────────────────────────────────
		internal sealed class DictionaryComboItem(int? id, string display)
		{
			public int? Id => id;
			public override string ToString() => display;
		}
	}
}

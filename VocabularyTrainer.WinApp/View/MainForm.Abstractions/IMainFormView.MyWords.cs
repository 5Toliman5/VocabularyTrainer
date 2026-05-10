using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		event EventHandler? MyWordsPageEntered;
		event EventHandler? ApplyWordFilterRequested;
		event EventHandler? PreviousWordPageRequested;
		event EventHandler? NextWordPageRequested;
		event EventHandler? DeleteMyWordsWordRequested;
		event EventHandler<string>? MyWordsSortChanged;
		event EventHandler? ResetWordFilterRequested;

		int? MyWordsDictionaryId { get; }
		string MyWordsLanguage { get; }
		string MyWordsSearch { get; }
		DateTime? MyWordsDateFrom { get; }
		DateTime? MyWordsDateTo { get; }
		WordDto SelectedMyWordsWord { get; }

		void LoadMyWordsPage(PagedResult<WordDto> result);
		void ResetMyWordsFilters();
	}
}

using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		event EventHandler? AddWordRequested;

		string InputWord { get; }
		IReadOnlyList<WordTranslationDto> InputTranslations { get; }
		int? SelectedAddingDictionaryId { get; }

		bool ValidateAddWordInput();
		void ClearAddWordInput();
		void ShowAddingDictionaryError(string message);
	}
}

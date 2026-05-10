using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		void ShowError(string message);
		void LoadDictionaries(IReadOnlyList<DictionaryDto> dictionaries);
	}
}

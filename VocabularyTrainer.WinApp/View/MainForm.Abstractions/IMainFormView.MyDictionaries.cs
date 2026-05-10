using System.Globalization;

namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		event EventHandler? AddDictionaryClickRequested;
		event EventHandler? UpdateDictionaryRequested;
		event EventHandler? DeleteDictionaryRequested;
		event EventHandler? MyDictionariesPageEntered;

		IWin32Window DialogOwner { get; }
		IReadOnlyList<CultureInfo> NeutralCultures { get; }

		int? SelectedMyWordsDictionaryId { get; }
		string InputDictionaryName { get; }
		string InputLanguageCode { get; }

		void ClearMyWordsDictionaryInput();
	}
}

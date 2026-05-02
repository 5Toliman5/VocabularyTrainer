namespace VocabularyTrainer.WinApp.View
{
	public class DictionaryInputEventArgs(string name, string? languageCode) : EventArgs
	{
		public string Name { get; } = name;
		public string? LanguageCode { get; } = languageCode;
	}
}

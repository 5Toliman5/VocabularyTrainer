namespace VocabularyTrainer.WinApp.View
{
	public class DictionaryInputEventArgs(string name, string? languageCode, string algorithmCode) : EventArgs
	{
		public string Name { get; } = name;
		public string LanguageCode { get; } = languageCode;
		public string AlgorithmCode { get; } = algorithmCode;
	}
}

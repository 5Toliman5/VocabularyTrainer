namespace VocabularyTrainer.WinApp.Infrastructure
{
	internal sealed class AlgorithmComboItem(string code, string display)
	{
		public string Code { get; } = code;
		public override string ToString() => display;
	}
}

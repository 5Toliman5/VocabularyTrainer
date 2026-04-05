namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	public class AppConfig
	{
		public string ConnectionString { get; init; } = string.Empty;
		public int MaxWordWeight { get; init; }
		public string LogsDirectory { get; init; } = "logs";
	}
}

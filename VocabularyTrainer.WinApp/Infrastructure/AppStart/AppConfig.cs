namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	/// <summary>Application configuration settings loaded from appsettings.json.</summary>
	public class AppConfig
	{
		/// <summary>Database connection string.</summary>
		public string ConnectionString { get; init; } = string.Empty;

		/// <summary>Maximum training weight a word can accumulate.</summary>
		public int MaxWordWeight { get; init; }

		/// <summary>Path to the directory where log files are written. Relative paths are resolved from the application base directory.</summary>
		public string LogsDirectory { get; init; } = "logs";
	}
}

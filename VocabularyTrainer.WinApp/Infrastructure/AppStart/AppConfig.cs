namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	/// <summary>Application configuration settings loaded from appsettings.json.</summary>
	public class AppConfig
	{
		/// <summary>Base URL of the VocabularyTrainer API (e.g. http://localhost:8080).</summary>
		public string ApiBaseUrl { get; init; } = string.Empty;

		/// <summary>Path to the directory where log files are written. Relative paths are resolved from the application base directory.</summary>
		public string LogsDirectory { get; init; } = "logs";
	}
}

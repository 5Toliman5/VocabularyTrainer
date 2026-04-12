using Microsoft.Extensions.Configuration;

namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	/// <summary>Loads and validates application configuration from the JSON settings file.</summary>
	public static class ConfigLoader
	{
		/// <summary>Reads <c>appsettings.json</c> from the application base directory and returns a validated <see cref="AppConfig"/>.</summary>
		/// <exception cref="InvalidOperationException">Thrown when required settings are missing or invalid.</exception>
		public static AppConfig Load()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
				.Build();

			var config = configuration.Get<AppConfig>();

			if (config is null || string.IsNullOrEmpty(config.ConnectionString))
				throw new InvalidOperationException("ConnectionString is not configured in appsettings.json.");

			if (config.MaxWordWeight <= 0)
				throw new InvalidOperationException("MaxWordWeight is not configured in appsettings.json.");

			return config;
		}
	}
}

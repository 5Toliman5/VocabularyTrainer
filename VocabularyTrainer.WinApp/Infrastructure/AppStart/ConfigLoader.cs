using Microsoft.Extensions.Configuration;

namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	public static class ConfigLoader
	{
		public static AppConfig Load()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
				.Build();

			var config = configuration.Get<AppConfig>();

			if (config is null || string.IsNullOrEmpty(config.ApiBaseUrl))
				throw new InvalidOperationException("ApiBaseUrl is not configured in appsettings.json.");

			return config;
		}
	}
}

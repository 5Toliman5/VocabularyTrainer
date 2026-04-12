using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using VocabularyTrainer.BusinessLogic;

namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	/// <summary>Extension methods for registering Vocabulary Trainer services with the DI container.</summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>Registers all application services and configures Serilog rolling-file logging.</summary>
		public static IServiceCollection AddVocabularyTrainerServices(
			this IServiceCollection services, AppConfig config)
		{
			ConfigureLogging(services, config.LogsDirectory);

			services.AddVocabularyTrainer(config.ConnectionString, config.MaxWordWeight);

			return services;
		}

		private static void ConfigureLogging(IServiceCollection services, string logsDirectory)
		{
			var logsPath = Path.IsPathRooted(logsDirectory)
				? logsDirectory
				: Path.Combine(AppContext.BaseDirectory, logsDirectory);

			Directory.CreateDirectory(logsPath);

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.File(
					path: Path.Combine(logsPath, "log-.txt"),
					rollingInterval: RollingInterval.Day,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			services.AddLogging(builder =>
			{
				builder.ClearProviders();
				builder.AddSerilog(dispose: true);
			});
		}
	}
}

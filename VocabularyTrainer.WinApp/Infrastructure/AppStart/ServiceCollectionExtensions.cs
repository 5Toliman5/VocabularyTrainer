using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using VocabularyTrainer.BusinessLogic.Services;
using VocabularyTrainer.DataAccess.Repositories;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddVocabularyTrainerServices(
			this IServiceCollection services, AppConfig config)
		{
			ConfigureLogging(services, config.LogsDirectory);

			services.AddSingleton<IUserRepository>(_ => new UserRepository(config.ConnectionString));
			services.AddSingleton<IWordRepository>(_ => new WordRepository(config.ConnectionString));
			services.AddSingleton<IDictionaryRepository>(_ => new DictionaryRepository(config.ConnectionString));

			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<IWordsShuffleService, WordsShuffleService>();
			services.AddSingleton<IDictionaryService, DictionaryService>();
			services.AddSingleton<IWordTrainerService>(provider =>
				new WordTrainerService(
					config.MaxWordWeight,
					provider.GetRequiredService<IWordRepository>(),
					provider.GetRequiredService<IWordsShuffleService>())
				);

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

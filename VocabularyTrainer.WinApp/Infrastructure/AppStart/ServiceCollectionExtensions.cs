using Microsoft.Extensions.DependencyInjection;
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
			services.AddSingleton<IUserRepository>(_ => new UserRepository(config.ConnectionString));
			services.AddSingleton<IWordRepository>(_ => new WordRepository(config.ConnectionString));

			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<IWordsShuffleService, WordsShuffleService>();
			services.AddSingleton<IWordTrainerService>(provider =>
				new WordTrainerService(
					config.MaxWordWeight,
					provider.GetRequiredService<IWordRepository>(),
					provider.GetRequiredService<IWordsShuffleService>())
				);

			return services;
		}
	}
}

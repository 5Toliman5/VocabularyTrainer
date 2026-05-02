using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.BusinessLogic.Services;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddVocabularyTrainer(
			this IServiceCollection services, string connectionString)
		{
			services.AddDataAccess(connectionString);
			return services.AddVocabularyTrainer();
		}

		public static IServiceCollection AddVocabularyTrainer(this IServiceCollection services)
		{
			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<IWordService, WordService>();
			services.AddSingleton<IWordsShuffleService, WordsShuffleService>();
			services.AddSingleton<IDictionaryService, DictionaryService>();
			services.AddSingleton<IWordTrainerService>(provider =>
				new WordTrainerService
				(
					provider.GetRequiredService<IWordRepository>(),
					provider.GetRequiredService<IWordsShuffleService>()
				)
			);

			return services;
		}
	}
}

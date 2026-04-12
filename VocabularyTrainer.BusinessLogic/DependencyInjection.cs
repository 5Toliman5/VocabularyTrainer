using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.BusinessLogic.Services;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic
{
	/// <summary>DI registration entry point for the BusinessLogic layer and its DataAccess dependencies.</summary>
	public static class DependencyInjection
	{
		/// <summary>Registers all business logic services and data access repositories.</summary>
		public static IServiceCollection AddVocabularyTrainer(
			this IServiceCollection services, string connectionString, int maxWordWeight)
		{
			services.AddDataAccess(connectionString);

			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<IWordsShuffleService, WordsShuffleService>();
			services.AddSingleton<IDictionaryService, DictionaryService>();
			services.AddSingleton<IWordTrainerService>(provider =>
				new WordTrainerService(
					maxWordWeight,
					provider.GetRequiredService<IWordRepository>(),
					provider.GetRequiredService<IWordsShuffleService>()));

			return services;
		}
	}
}

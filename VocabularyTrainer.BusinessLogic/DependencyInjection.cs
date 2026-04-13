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
			this IServiceCollection services, string connectionString)
		{
			services.AddDataAccess(connectionString);
			return services.AddVocabularyTrainer();
		}

		/// <summary>Registers all business logic services. Repository implementations must be registered separately before calling this.</summary>
		public static IServiceCollection AddVocabularyTrainer(this IServiceCollection services)
		{
			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<IWordsShuffleService, WordsShuffleService>();
			services.AddSingleton<IDictionaryService, DictionaryService>();
			services.AddSingleton<IWordTrainerService>(provider =>
				new WordTrainerService(
					provider.GetRequiredService<IWordRepository>(),
					provider.GetRequiredService<IWordsShuffleService>()));

			return services;
		}
	}
}

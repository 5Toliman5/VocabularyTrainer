using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.BusinessLogic.Services;
using VocabularyTrainer.BusinessLogic.Services.Algorithms;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddVocabularyTrainerForApi(
			this IServiceCollection services, string connectionString)
		{
			services.AddDataAccess(connectionString);
			services.AddVocabularyTrainerCoreScoped();
			services.AddTrainingAlgorithmsScoped();
			services.AddScoped<ITrainingService, TrainingService>();
			services.AddScoped<AlgorithmsSeeder>();

			return services;
		}

		public static IServiceCollection AddVocabularyTrainerForApiClient(this IServiceCollection services)
		{
			services.AddVocabularyTrainerCoreSingleton();
			services.AddSingleton<IWordTrainerService, WordTrainerService>();

			return services;
		}

		private static IServiceCollection AddVocabularyTrainerCoreScoped(this IServiceCollection services)
		{
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IWordService, WordService>();
			services.AddScoped<IDictionaryService, DictionaryService>();

			return services;
		}

		private static IServiceCollection AddVocabularyTrainerCoreSingleton(this IServiceCollection services)
		{
			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<IWordService, WordService>();
			services.AddSingleton<IDictionaryService, DictionaryService>();

			return services;
		}

		private static IServiceCollection AddTrainingAlgorithmsScoped(this IServiceCollection services)
		{
			services.AddScoped<IWordTrainingAlgorithm, WeightBasedAlgorithm>();
			services.AddScoped<IWordTrainingAlgorithm, Sm2Algorithm>();
			services.AddScoped<ITrainingAlgorithmRegistry, TrainingAlgorithmRegistry>();

			return services;
		}
	}
}

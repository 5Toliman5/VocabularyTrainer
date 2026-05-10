using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.Api.BusinessLogic.Services;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.BusinessLogic;

namespace VocabularyTrainer.Api.BusinessLogic
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApiBusinessLogic(
			this IServiceCollection services, string connectionString)
		{
			services.AddVocabularyTrainerForApi(connectionString);

			services.AddScoped<IApiUserService, ApiUserService>();
			services.AddScoped<IApiDictionaryService, ApiDictionaryService>();
			services.AddScoped<IApiWordService, ApiWordService>();
			services.AddScoped<IApiTrainingService, ApiTrainingService>();
			services.AddScoped<IApiAlgorithmService, ApiAlgorithmService>();

			return services;
		}
	}
}

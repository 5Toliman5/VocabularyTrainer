using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.Api.BusinessLogic.Services;
using VocabularyTrainer.BusinessLogic;

namespace VocabularyTrainer.Api.BusinessLogic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiBusinessLogic(
            this IServiceCollection services, string connectionString)
        {
            services.AddVocabularyTrainer(connectionString);
            services.AddSingleton<IApiDictionaryService, ApiDictionaryService>();
            services.AddSingleton<IApiUserService, ApiUserService>();
            services.AddSingleton<IApiWordService, ApiWordService>();
            return services;
        }
    }
}

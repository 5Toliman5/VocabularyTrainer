using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.WinApp.ApiClient.Mapping;
using VocabularyTrainer.WinApp.ApiClient.Repositories;

namespace VocabularyTrainer.WinApp.ApiClient
{
    /// <summary>DI registration entry point for the HTTP API client layer.</summary>
    public static class DependencyInjection
    {
        /// <summary>Registers HTTP repository implementations and AutoMapper mappings pointed at the given API base URL.</summary>
        public static IServiceCollection AddApiClient(this IServiceCollection services, string baseUrl)
        {
            services.AddSingleton(new HttpClient { BaseAddress = new Uri(baseUrl) });
            services.AddSingleton(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper());

            services.AddSingleton<IUserRepository, UserApiRepository>();
            services.AddSingleton<IWordRepository, WordApiRepository>();
            services.AddSingleton<IDictionaryRepository, DictionaryApiRepository>();

            return services;
        }
    }
}

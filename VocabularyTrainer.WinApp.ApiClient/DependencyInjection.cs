using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;
using VocabularyTrainer.WinApp.ApiClient.Auth;
using VocabularyTrainer.WinApp.ApiClient.Mapping;
using VocabularyTrainer.WinApp.ApiClient.Repositories;
using CClientInfoDto = VocabularyTrainer.Api.Contract.Auth.ClientInfoDto;

namespace VocabularyTrainer.WinApp.ApiClient
{
	public static class DependencyInjection
	{
		private static readonly TimeSpan AuthRequestTimeout = TimeSpan.FromSeconds(15);

		// The host must register an ITokenStore.
		public static IServiceCollection AddApiClient(this IServiceCollection services, string baseUrl, string clientType)
		{
			var baseAddress = new Uri(baseUrl);
			var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

			services.AddSingleton(mapper);

			// Short timeout: session restore runs before any window is shown, so a hung API must fail fast.
			var authHttpClient = new HttpClient { BaseAddress = baseAddress, Timeout = AuthRequestTimeout };
			var clientInfo = new CClientInfoDto(clientType, Environment.MachineName);

			services.AddSingleton(sp => new AccountClient(authHttpClient, sp.GetRequiredService<ITokenStore>(), mapper, clientInfo));
			services.AddSingleton<IAccountClient>(sp => sp.GetRequiredService<AccountClient>());

			services.AddSingleton(sp => CreateAuthenticatedHttpClient(sp.GetRequiredService<AccountClient>(), baseAddress));
			services.AddSingleton<IAccountSettingsClient, AccountSettingsClient>();

			services.AddSingleton<IWordRepository, WordApiRepository>();
			services.AddSingleton<IDictionaryRepository, DictionaryApiRepository>();

			services.AddSingleton<ITrainingClient, HttpTrainingClient>();
			services.AddSingleton<IAlgorithmCatalog, HttpAlgorithmCatalog>();

			return services;
		}

		private static HttpClient CreateAuthenticatedHttpClient(AccountClient accountClient, Uri baseAddress)
		{
			var handler = new AuthHeaderHandler(accountClient)
			{
				InnerHandler = new HttpClientHandler(),
			};

			return new HttpClient(handler) { BaseAddress = baseAddress };
		}
	}
}

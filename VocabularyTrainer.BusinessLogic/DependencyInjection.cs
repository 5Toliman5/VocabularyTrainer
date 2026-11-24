using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.BusinessLogic.Services;
using VocabularyTrainer.BusinessLogic.Services.Algorithms;
using VocabularyTrainer.BusinessLogic.Services.Auth;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic
{
	public static class DependencyInjection
	{
		// The host must also register ITokenService and IEmailSender, and bind AuthOptions.
		public static IServiceCollection AddVocabularyTrainerForApi(
			this IServiceCollection services, string connectionString)
		{
			services.AddDataAccess(connectionString);
			services.AddVocabularyTrainerCoreScoped();
			services.AddTrainingAlgorithmsScoped();
			services.AddAuthScoped();
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
			services.AddScoped<IWordService, WordService>();
			services.AddScoped<IDictionaryService, DictionaryService>();

			return services;
		}

		private static IServiceCollection AddVocabularyTrainerCoreSingleton(this IServiceCollection services)
		{
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

		// New sign-in providers: register another IExternalAuthProvider.
		// New code delivery channels (Telegram, Viber for phones): register another IVerificationCodeSender.
		private static IServiceCollection AddAuthScoped(this IServiceCollection services)
		{
			services.AddOptions<AuthOptions>();

			services.AddSingleton<IPasswordHasher, PasswordHasher>();
			services.AddScoped<IVerificationCodeSender, EmailVerificationCodeSender>();
			services.AddScoped<IVerificationCodeService, VerificationCodeService>();
			services.AddScoped<IExternalAuthProviderRegistry, ExternalAuthProviderRegistry>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IAuthMaintenanceService, AuthMaintenanceService>();

			return services;
		}
	}
}

using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.DataAccess.Repositories;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess
{
	/// <summary>DI registration entry point for the DataAccess layer.</summary>
	public static class DependencyInjection
	{
		/// <summary>Registers all repository implementations with the provided connection string.</summary>
		public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
		{
			services.AddSingleton<IUserRepository>(_ => new UserRepository(connectionString));
			services.AddSingleton<IWordRepository>(_ => new WordRepository(connectionString));
			services.AddSingleton<IDictionaryRepository>(_ => new DictionaryRepository(connectionString));

			return services;
		}
	}
}

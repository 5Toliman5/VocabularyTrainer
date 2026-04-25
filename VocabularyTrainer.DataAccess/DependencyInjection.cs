using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.DataAccess.Repositories;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
		{
			services.AddSingleton<IUserRepository>(_ => new UserRepository(connectionString));
			services.AddSingleton<IWordRepository>(_ => new WordRepository(connectionString));
			services.AddSingleton<IDictionaryRepository>(_ => new DictionaryRepository(connectionString));

			return services;
		}
	}
}

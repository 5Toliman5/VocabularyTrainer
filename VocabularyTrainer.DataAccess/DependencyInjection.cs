using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.DataAccess.Repositories;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
		{
			services.AddDbContext<VocabularyTrainerDbContext>(opts => opts.UseSqlServer(connectionString));
			services.AddScoped<IVocabularyTrainerDbContext>(sp => sp.GetRequiredService<VocabularyTrainerDbContext>());

			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IAlgorithmRepository, AlgorithmRepository>();
			services.AddScoped<IDictionaryRepository, DictionaryRepository>();
			services.AddScoped<IWordRepository, WordRepository>();
			services.AddScoped<IWordWeightBasedParamsRepository, WordWeightBasedParamsRepository>();
			services.AddScoped<IWordSm2ParamsRepository, WordSm2ParamsRepository>();

			return services;
		}
	}
}

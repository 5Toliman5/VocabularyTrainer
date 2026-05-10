using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.DataAccess.Configurations;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess
{
	public class VocabularyTrainerDbContext : DbContext, IVocabularyTrainerDbContext
	{
		public DbSet<User> Users => Set<User>();
		public DbSet<Algorithm> Algorithms => Set<Algorithm>();
		public DbSet<UserDictionary> Dictionaries => Set<UserDictionary>();
		public DbSet<Word> Words => Set<Word>();
		public DbSet<WordTranslation> WordTranslations => Set<WordTranslation>();
		public DbSet<WordWeightBasedParams> WordWeightBasedParams => Set<WordWeightBasedParams>();
		public DbSet<WordSm2Params> WordSm2Params => Set<WordSm2Params>();

		public VocabularyTrainerDbContext(DbContextOptions<VocabularyTrainerDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new UserConfiguration());
			modelBuilder.ApplyConfiguration(new AlgorithmConfiguration());
			modelBuilder.ApplyConfiguration(new UserDictionaryConfiguration());
			modelBuilder.ApplyConfiguration(new WordConfiguration());
			modelBuilder.ApplyConfiguration(new WordTranslationConfiguration());
			modelBuilder.ApplyConfiguration(new WordWeightBasedParamsConfiguration());
			modelBuilder.ApplyConfiguration(new WordSm2ParamsConfiguration());
		}
	}
}

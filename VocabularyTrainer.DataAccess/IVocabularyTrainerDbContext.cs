using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess
{
	public interface IVocabularyTrainerDbContext
	{
		DbSet<User> Users { get; }
		DbSet<Algorithm> Algorithms { get; }
		DbSet<UserDictionary> Dictionaries { get; }
		DbSet<Word> Words { get; }
		DbSet<WordTranslation> WordTranslations { get; }
		DbSet<WordWeightBasedParams> WordWeightBasedParams { get; }
		DbSet<WordSm2Params> WordSm2Params { get; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}

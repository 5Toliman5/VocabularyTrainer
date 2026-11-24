using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess
{
	public interface IVocabularyTrainerDbContext
	{
		DbSet<User> Users { get; }
		DbSet<UserIdentity> UserIdentities { get; }
		DbSet<RefreshToken> RefreshTokens { get; }
		DbSet<VerificationCode> VerificationCodes { get; }
		DbSet<Algorithm> Algorithms { get; }
		DbSet<UserDictionary> Dictionaries { get; }
		DbSet<Word> Words { get; }
		DbSet<WordTranslation> WordTranslations { get; }
		DbSet<WordWeightBasedParams> WordWeightBasedParams { get; }
		DbSet<WordSm2Params> WordSm2Params { get; }

		DatabaseFacade Database { get; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}

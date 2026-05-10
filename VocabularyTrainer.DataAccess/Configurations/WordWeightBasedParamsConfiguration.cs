using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class WordWeightBasedParamsConfiguration : IEntityTypeConfiguration<WordWeightBasedParams>
	{
		public void Configure(EntityTypeBuilder<WordWeightBasedParams> builder)
		{
			builder.ToTable("WordWeightBasedParams");
			builder.HasKey(p => p.WordId);
			builder.Property(p => p.WordId).ValueGeneratedNever();

			builder.Property(p => p.Weight).IsRequired().HasDefaultValue(0);
		}
	}
}

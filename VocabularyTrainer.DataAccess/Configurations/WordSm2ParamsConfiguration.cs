using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class WordSm2ParamsConfiguration : IEntityTypeConfiguration<WordSm2Params>
	{
		public void Configure(EntityTypeBuilder<WordSm2Params> builder)
		{
			builder.ToTable("WordSm2Params");
			builder.HasKey(p => p.WordId);
			builder.Property(p => p.WordId).ValueGeneratedNever();

			builder.Property(p => p.Repetitions)   .IsRequired().HasDefaultValue(0);
			builder.Property(p => p.IntervalDays)  .IsRequired().HasDefaultValue(0);
			builder.Property(p => p.EaseFactor)    .HasColumnType("decimal(4,2)").IsRequired().HasDefaultValue(2.50m);
			builder.Property(p => p.Lapses)        .IsRequired().HasDefaultValue(0);
			builder.Property(p => p.LastReviewedAt);
			builder.Property(p => p.NextDueAt);

			builder.HasIndex(p => p.NextDueAt).HasDatabaseName("IX_WordSm2Params_NextDueAt");
		}
	}
}

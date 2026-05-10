using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class WordTranslationConfiguration : IEntityTypeConfiguration<WordTranslation>
	{
		public void Configure(EntityTypeBuilder<WordTranslation> builder)
		{
			builder.ToTable("WordTranslations");
			builder.HasKey(t => t.Id);
			builder.Property(t => t.Id).HasColumnName("ID");

			builder.Property(t => t.WordId).IsRequired();
			builder.Property(t => t.Text)  .HasMaxLength(500).IsRequired();

			// Persist enum as its name (nvarchar) for human-readable rows in DB
			// and stable storage independent of enum integer values.
			builder.Property(t => t.Kind)
			       .HasConversion<string>()
			       .HasMaxLength(20)
			       .IsRequired()
			       .HasDefaultValue(TranslationKind.Translation);

			builder.HasIndex(t => t.WordId).HasDatabaseName("IX_WordTranslations_WordId");
		}
	}
}

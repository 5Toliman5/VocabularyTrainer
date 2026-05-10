using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class WordConfiguration : IEntityTypeConfiguration<Word>
	{
		public void Configure(EntityTypeBuilder<Word> builder)
		{
			builder.ToTable("Words");
			builder.HasKey(w => w.Id);
			builder.Property(w => w.Id).HasColumnName("ID");

			builder.Property(w => w.UserId)         .IsRequired();
			builder.Property(w => w.DictionaryId)   .IsRequired();
			builder.Property(w => w.Value)          .HasMaxLength(200).IsRequired();
			builder.Property(w => w.NormalizedText) .HasMaxLength(200).IsRequired();
			builder.Property(w => w.LanguageCode)   .HasMaxLength(10) .IsRequired();

			builder.Property(w => w.DateAdded)   .HasDefaultValueSql("GETUTCDATE()");
			builder.Property(w => w.DateModified).HasDefaultValueSql("GETUTCDATE()");

			builder.HasIndex(w => new { w.UserId, w.DictionaryId, w.NormalizedText })
			       .IsUnique()
			       .HasDatabaseName("UQ_Words_User_Dict_Normalized");

			builder.HasIndex(w => w.UserId)         .HasDatabaseName("IX_Words_UserId");
			builder.HasIndex(w => w.DictionaryId)   .HasDatabaseName("IX_Words_DictionaryId");
			builder.HasIndex(w => w.DateAdded)      .HasDatabaseName("IX_Words_DateAdded");
			builder.HasIndex(w => w.NormalizedText) .HasDatabaseName("IX_Words_NormalizedText");

			builder.HasOne(w => w.User)
			       .WithMany(u => u.Words)
			       .HasForeignKey(w => w.UserId)
			       .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(w => w.Dictionary)
			       .WithMany(d => d.Words)
			       .HasForeignKey(w => w.DictionaryId)
			       .OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(w => w.Translations)
			       .WithOne(t => t.Word)
			       .HasForeignKey(t => t.WordId)
			       .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(w => w.WeightBasedParams)
			       .WithOne(p => p.Word)
			       .HasForeignKey<WordWeightBasedParams>(p => p.WordId)
			       .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(w => w.Sm2Params)
			       .WithOne(p => p.Word)
			       .HasForeignKey<WordSm2Params>(p => p.WordId)
			       .OnDelete(DeleteBehavior.Cascade);
		}
	}
}

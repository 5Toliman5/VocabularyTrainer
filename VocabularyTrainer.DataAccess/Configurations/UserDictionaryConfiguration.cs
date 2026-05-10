using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class UserDictionaryConfiguration : IEntityTypeConfiguration<UserDictionary>
	{
		public void Configure(EntityTypeBuilder<UserDictionary> builder)
		{
			builder.ToTable("Dictionaries");
			builder.HasKey(d => d.Id);
			builder.Property(d => d.Id).HasColumnName("ID");

			builder.Property(d => d.UserId)       .IsRequired();
			builder.Property(d => d.Name)         .HasMaxLength(50).IsRequired();
			builder.Property(d => d.LanguageCode) .HasMaxLength(10);
			builder.Property(d => d.AlgorithmId)  .IsRequired();

			builder.HasIndex(d => new { d.UserId, d.Name })
			       .IsUnique()
			       .HasDatabaseName("UQ_Dictionaries_UserId_Name");

			builder.HasIndex(d => d.AlgorithmId)
			       .HasDatabaseName("IX_Dictionaries_AlgorithmId");

			builder.HasOne(d => d.User)
			       .WithMany(u => u.Dictionaries)
			       .HasForeignKey(d => d.UserId)
			       .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(d => d.Algorithm)
			       .WithMany(a => a.Dictionaries)
			       .HasForeignKey(d => d.AlgorithmId)
			       .OnDelete(DeleteBehavior.Restrict);
		}
	}
}

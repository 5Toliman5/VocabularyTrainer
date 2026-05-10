using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class AlgorithmConfiguration : IEntityTypeConfiguration<Algorithm>
	{
		public void Configure(EntityTypeBuilder<Algorithm> builder)
		{
			builder.ToTable("Algorithms");
			builder.HasKey(a => a.Id);
			builder.Property(a => a.Id).HasColumnName("ID");

			builder.Property(a => a.Code).HasMaxLength(20).IsRequired();

			builder.HasIndex(a => a.Code)
			       .IsUnique()
			       .HasDatabaseName("UQ_Algorithms_Code");
		}
	}
}

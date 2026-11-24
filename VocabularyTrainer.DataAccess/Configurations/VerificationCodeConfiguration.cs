using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
	{
		public void Configure(EntityTypeBuilder<VerificationCode> builder)
		{
			builder.ToTable("VerificationCodes");
			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).HasColumnName("ID");

			builder.Property(c => c.Channel).HasMaxLength(20).IsRequired();
			builder.Property(c => c.Target).HasMaxLength(256).IsRequired();
			builder.Property(c => c.Purpose).HasConversion<string>().HasMaxLength(20).IsRequired();
			builder.Property(c => c.CodeHash).HasColumnType("binary(32)").IsRequired();

			builder.HasIndex(c => new { c.Channel, c.Target, c.Purpose, c.CreatedAt })
				.HasDatabaseName("IX_VerificationCodes_Target");
		}
	}
}

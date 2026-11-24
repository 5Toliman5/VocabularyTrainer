using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
	{
		public void Configure(EntityTypeBuilder<RefreshToken> builder)
		{
			builder.ToTable("RefreshTokens");
			builder.HasKey(t => t.Id);
			builder.Property(t => t.Id).HasColumnName("ID");

			builder.Property(t => t.TokenHash).HasColumnType("binary(32)").IsRequired();
			builder.Property(t => t.ClientType).HasMaxLength(20).IsRequired();
			builder.Property(t => t.DeviceName).HasMaxLength(128);

			builder.HasIndex(t => t.TokenHash)
				.IsUnique()
				.HasDatabaseName("UQ_RefreshTokens_TokenHash");

			builder.HasOne(t => t.User)
				.WithMany()
				.HasForeignKey(t => t.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}

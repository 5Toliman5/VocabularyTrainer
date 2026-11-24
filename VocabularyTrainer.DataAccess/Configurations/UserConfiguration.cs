using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users");
			builder.HasKey(u => u.Id);
			builder.Property(u => u.Id).HasColumnName("ID");

			builder.Property(u => u.DisplayName).HasMaxLength(64).IsRequired();
			builder.Property(u => u.PasswordHash).HasMaxLength(256);
			builder.Property(u => u.CreatedAt).IsRequired();
			builder.Property(u => u.IsBlocked).IsRequired();
		}
	}
}

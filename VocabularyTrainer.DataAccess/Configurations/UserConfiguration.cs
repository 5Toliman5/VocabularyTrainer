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

			builder.Property(u => u.Name)    .HasMaxLength(64) .IsRequired();
			builder.Property(u => u.Login)   .HasMaxLength(32) .IsRequired();
			builder.Property(u => u.Password).HasMaxLength(256).IsRequired();
			builder.Property(u => u.Info);

			builder.HasIndex(u => u.Name).IsUnique();
		}
	}
}

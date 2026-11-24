using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.DataAccess.Configurations
{
	internal class UserIdentityConfiguration : IEntityTypeConfiguration<UserIdentity>
	{
		public void Configure(EntityTypeBuilder<UserIdentity> builder)
		{
			builder.ToTable("UserIdentities");
			builder.HasKey(i => i.Id);
			builder.Property(i => i.Id).HasColumnName("ID");

			builder.Property(i => i.Provider).HasMaxLength(20).IsRequired();
			builder.Property(i => i.ProviderKey).HasMaxLength(256).IsRequired();
			builder.Property(i => i.CreatedAt).IsRequired();

			builder.HasIndex(i => new { i.Provider, i.ProviderKey })
				.IsUnique()
				.HasDatabaseName("UQ_UserIdentities_Provider_ProviderKey");

			builder.HasOne(i => i.User)
				.WithMany(u => u.Identities)
				.HasForeignKey(i => i.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}

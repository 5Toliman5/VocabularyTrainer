using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class UserIdentity : EntityBase
	{
		public int UserId { get; set; }

		[Required, MaxLength(20)]
		public string Provider { get; set; } = null!;

		[Required, MaxLength(256)]
		public string ProviderKey { get; set; } = null!;

		public DateTime CreatedAt { get; set; }

		public DateTime? LastLoginAt { get; set; }

		public User User { get; set; } = null!;
	}
}

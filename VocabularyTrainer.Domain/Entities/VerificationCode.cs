using System.ComponentModel.DataAnnotations;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Entities
{
	public class VerificationCode : EntityBase
	{
		[Required, MaxLength(20)]
		public string Channel { get; set; } = null!;

		[Required, MaxLength(256)]
		public string Target { get; set; } = null!;

		public VerificationPurpose Purpose { get; set; }

		public byte[] CodeHash { get; set; } = null!;

		public DateTime CreatedAt { get; set; }

		public DateTime ExpiresAt { get; set; }

		public int AttemptCount { get; set; }

		public DateTime? ConsumedAt { get; set; }
	}
}

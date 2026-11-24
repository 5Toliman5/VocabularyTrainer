using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class RefreshToken : EntityBase
	{
		public int UserId { get; set; }

		public byte[] TokenHash { get; set; } = null!;

		[Required, MaxLength(20)]
		public string ClientType { get; set; } = null!;

		[MaxLength(128)]
		public string? DeviceName { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime ExpiresAt { get; set; }

		public DateTime? RevokedAt { get; set; }

		// Set when the token is rotated; presenting a token that already has a successor means it was stolen.
		public int? ReplacedById { get; set; }

		public User User { get; set; } = null!;
	}
}

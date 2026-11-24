using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class User : EntityBase
	{
		[Required, MaxLength(64)]
		public string DisplayName { get; set; } = null!;

		// Null for accounts created through an external provider that never set a password.
		[MaxLength(256)]
		public string? PasswordHash { get; set; }

		public DateTime CreatedAt { get; set; }

		public bool IsBlocked { get; set; }

		public ICollection<UserIdentity> Identities { get; set; } = [];
		public ICollection<UserDictionary> Dictionaries { get; set; } = [];
		public ICollection<Word> Words { get; set; } = [];
	}
}

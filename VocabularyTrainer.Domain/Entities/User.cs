using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	/// <summary>Represents an application user.</summary>
	public class User : EntityBase
	{
		/// <summary>Display name of the user.</summary>
		[Required, MaxLength(64)]
		public string Name { get; set; } = null!;

		/// <summary>Optional free-form information about the user.</summary>
		public string? Info { get; set; }

		/// <summary>Login identifier used for authentication.</summary>
		[Required, MaxLength(32)]
		public string Login { get; set; } = null!;

		/// <summary>Hashed password for authentication.</summary>
		[Required, MaxLength(256)]
		public string Password { get; set; } = null!;

		/// <summary>Words associated with this user across all dictionaries.</summary>
		public ICollection<UserWord> UserWords { get; } = [];
	}
}

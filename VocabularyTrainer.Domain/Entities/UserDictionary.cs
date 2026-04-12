using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	/// <summary>Represents a named vocabulary dictionary belonging to a user.</summary>
	public class UserDictionary : EntityBase
	{
		/// <summary>Foreign key to the owning user.</summary>
		[Required]
		public int UserId { get; set; }

		/// <summary>Name of the dictionary, unique per user.</summary>
		[Required]
		[MaxLength(50)]
		public string Name { get; set; } = null!;

		/// <summary>BCP 47 language code describing the target language (e.g. "en", "fr-FR").</summary>
		[MaxLength(10)]
		public string? LanguageCode { get; set; }

		/// <summary>Navigation property to the owning user.</summary>
		public User User { get; set; } = null!;

		/// <summary>Words stored in this dictionary.</summary>
		public ICollection<UserWord> UserWords { get; set; } = [];
	}
}

using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	/// <summary>Junction entity linking a user, a word, and a dictionary, and tracking the word's training weight.</summary>
	public class UserWord : EntityBase
	{
		/// <summary>Foreign key to the owning user.</summary>
		[Required]
		public int UserId { get; set; }

		/// <summary>Foreign key to the associated word.</summary>
		[Required]
		public int WordId { get; set; }

		/// <summary>Foreign key to the dictionary this word belongs to for the user.</summary>
		[Required]
		public int DictionaryId { get; set; }

		/// <summary>Training weight indicating how well the user knows this word. Higher values mean the word is shown less frequently.</summary>
		[Required]
		public int Weight { get; set; } = 0;

		/// <summary>Navigation property to the owning user.</summary>
		public User User { get; set; } = null!;

		/// <summary>Navigation property to the associated word.</summary>
		public Word Word { get; set; } = null!;

		/// <summary>Navigation property to the dictionary.</summary>
		public UserDictionary Dictionary { get; set; } = null!;
	}
}

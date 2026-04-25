using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class UserWord : EntityBase
	{
		[Required]
		public int UserId { get; set; }

		[Required]
		public int WordId { get; set; }

		[Required]
		public int DictionaryId { get; set; }

		// higher = shown less often
		[Required]
		public int Weight { get; set; } = 0;

		public User User { get; set; } = null!;

		public Word Word { get; set; } = null!;

		public UserDictionary Dictionary { get; set; } = null!;
	}
}

using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class UserDictionary : EntityBase
	{
		[Required]
		public int UserId { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; } = null!;

		[MaxLength(10)]
		public string? LanguageCode { get; set; }

		public User User { get; set; } = null!;

		public ICollection<UserWord> UserWords { get; set; } = [];
	}
}

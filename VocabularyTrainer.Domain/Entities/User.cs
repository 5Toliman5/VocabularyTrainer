using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class User : EntityBase
	{
		[Required, MaxLength(64)]
		public string Name { get; set; } = null!;

		public string? Info { get; set; }

		[Required, MaxLength(32)]
		public string Login { get; set; } = null!;

		[Required, MaxLength(32)]
		public string Password { get; set; } = null!;

		public ICollection<UserWord> UserWords { get; } = [];
	}
}

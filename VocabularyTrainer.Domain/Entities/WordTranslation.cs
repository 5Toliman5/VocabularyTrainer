using System.ComponentModel.DataAnnotations;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Entities
{
	public class WordTranslation : EntityBase
	{
		[Required]
		public int WordId { get; set; }

		[Required, MaxLength(500)]
		public string Text { get; set; } = null!;

		public TranslationKind Kind { get; set; } = TranslationKind.Translation;

		public Word Word { get; set; } = null!;
	}
}

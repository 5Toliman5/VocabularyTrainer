using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class Word : EntityBase
	{
		[Required]
		public int UserId { get; set; }

		[Required]
		public int DictionaryId { get; set; }

		[Required, MaxLength(200)]
		public string Value { get; set; } = null!;

		[Required, MaxLength(200)]
		public string NormalizedText { get; set; } = null!;

		[Required, MaxLength(10)]
		public string LanguageCode { get; set; } = null!;

		public DateTime DateAdded { get; set; }
		public DateTime DateModified { get; set; }

		public User User { get; set; } = null!;
		public UserDictionary Dictionary { get; set; } = null!;

		public ICollection<WordTranslation> Translations { get; set; } = [];
		public WordWeightBasedParams? WeightBasedParams { get; set; }
		public WordSm2Params? Sm2Params { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class Word : EntityBase
	{
		[Required, MaxLength(100)]
		public string Value { get; set; } = null!;

		[Required, MaxLength(100)]
		public string Translation { get; set; } = null!;

		public Word(string value, string translation)
		{
			Value = value;
			Translation = translation;
		}
	}
}

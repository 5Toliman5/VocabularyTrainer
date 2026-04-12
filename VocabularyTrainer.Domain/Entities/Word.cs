using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	/// <summary>Represents a word-translation pair stored in the shared word pool.</summary>
	public class Word : EntityBase
	{
		/// <summary>The word text.</summary>
		[Required, MaxLength(100)]
		public string Value { get; set; } = null!;

		/// <summary>The translation of the word.</summary>
		[Required, MaxLength(100)]
		public string Translation { get; set; } = null!;

		/// <summary>Initializes a new word with the specified value and translation.</summary>
		public Word(string value, string translation)
		{
			Value = value;
			Translation = translation;
		}
	}
}

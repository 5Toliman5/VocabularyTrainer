namespace VocabularyTrainer.Domain.Models
{
	/// <summary>Read model representing a word with its training weight and dictionary context.</summary>
	public record WordDto
	{
		/// <summary>Database identifier of the word.</summary>
		public int Id { get; init; }

		/// <summary>The word text.</summary>
		public string Value { get; init; }

		/// <summary>The translation of the word.</summary>
		public string Translation { get; init; }

		/// <summary>Training weight of the word for the current user and dictionary.</summary>
		public int Weight { get; init; }

		/// <summary>Identifier of the dictionary this word belongs to.</summary>
		public int DictionaryId { get; init; }

		/// <summary>Display name of the dictionary this word belongs to.</summary>
		public string DictionaryName { get; init; }

		/// <summary>Initializes a minimal word DTO with only value and translation, suitable for adding new words.</summary>
		public WordDto(string value, string translation)
		{
			Value = value;
			Translation = translation;
			DictionaryName = string.Empty;
		}

		/// <summary>Initializes a full word DTO as returned from the database.</summary>
		public WordDto(int id, string value, string translation, int weight, int dictionaryId, string dictionaryName)
			: this(value, translation)
		{
			Id = id;
			Weight = weight;
			DictionaryId = dictionaryId;
			DictionaryName = dictionaryName;
		}
	}
}

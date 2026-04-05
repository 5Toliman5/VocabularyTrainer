namespace VocabularyTrainer.Domain.Models
{
	public record WordDto
	{
		public int Id { get; init; }

		public string Value { get; init; }

		public string Translation { get; init; }

		public int Weight { get; init; }

		public int DictionaryId { get; init; }

		public string DictionaryName { get; init; }

		public WordDto(string value, string translation)
		{
			Value = value;
			Translation = translation;
			DictionaryName = string.Empty;
		}

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

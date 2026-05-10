namespace VocabularyTrainer.Domain.Models
{
	public record DictionaryDto
	(
		int Id,
		string Name,
		string? LanguageCode,
		string AlgorithmCode,
		int WordCount = 0
	)
	{
		public override string ToString() =>
			string.IsNullOrEmpty(LanguageCode) ? Name : $"{Name} ({LanguageCode})";
	}
}

namespace VocabularyTrainer.Domain.Models
{
	public record DictionaryDto
	(
		int Id,
		string Name,
		string? LanguageCode,
		int WordCount = 0
	)
	{
		public override string ToString() =>
			string.IsNullOrEmpty(LanguageCode) ? Name : $"{Name} ({LanguageCode})";
	}
}

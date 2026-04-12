namespace VocabularyTrainer.Domain.Models
{
	/// <summary>Read model representing a user dictionary with its word count.</summary>
	public record DictionaryDto(int Id, string Name, string? LanguageCode, int WordCount = 0)
	{
		/// <summary>Returns the dictionary name, appended with the language code in parentheses when present.</summary>
		public override string ToString() =>
			string.IsNullOrEmpty(LanguageCode) ? Name : $"{Name} ({LanguageCode})";
	}
}

namespace VocabularyTrainer.Domain.Models
{
	/// <summary>Request data for creating a new user dictionary.</summary>
	public record AddDictionaryRequest(int UserId, string Name, string? LanguageCode);
}

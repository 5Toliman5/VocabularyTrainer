namespace VocabularyTrainer.Domain.Models
{
	/// <summary>Request data for renaming or updating a user dictionary.</summary>
	public record UpdateDictionaryRequest(int DictionaryId, int UserId, string Name, string? LanguageCode);
}

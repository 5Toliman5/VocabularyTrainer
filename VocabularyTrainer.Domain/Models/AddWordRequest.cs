namespace VocabularyTrainer.Domain.Models
{
	/// <summary>Request data for adding a word to a user's dictionary.</summary>
	public record AddWordRequest(string Value, string Translation, int UserId, int DictionaryId);
}

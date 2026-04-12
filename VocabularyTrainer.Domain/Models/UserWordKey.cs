namespace VocabularyTrainer.Domain.Models
{
	/// <summary>Composite key that uniquely identifies a word entry within a specific user's dictionary.</summary>
	public record UserWordKey(int WordId, int UserId, int DictionaryId);
}

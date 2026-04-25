namespace VocabularyTrainer.Domain.Models
{
	public record UserWordKey
	(
		int WordId,
		int UserId,
		int DictionaryId
	);
}

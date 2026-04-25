namespace VocabularyTrainer.Domain.Models
{
	public record UpdateWordWeightRequest
	(
		int WordId,
		int UserId,
		int DictionaryId,
		UpdateWeightType Type
	) : UserWordKey(WordId, UserId, DictionaryId);

	public enum UpdateWeightType
	{
		Increase,
		Decrease
	}
}

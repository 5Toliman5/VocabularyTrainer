namespace VocabularyTrainer.Domain.Models
{
	/// <summary>Request data for updating the training weight of a word.</summary>
	public record UpdateWordWeightRequest(int WordId, int UserId, int DictionaryId, UpdateWeightType Type)
		: UserWordKey(WordId, UserId, DictionaryId);

	/// <summary>Specifies the direction of a training weight update.</summary>
	public enum UpdateWeightType
	{
		/// <summary>Increase the word's weight (user answered correctly).</summary>
		Increase,
		/// <summary>Decrease the word's weight (user answered incorrectly or skipped).</summary>
		Decrease
	}
}

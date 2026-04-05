namespace VocabularyTrainer.Domain.Models
{
	public record EditWordRequest(int WordId, int UserId, int DictionaryId);
}

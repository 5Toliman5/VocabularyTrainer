namespace VocabularyTrainer.Domain.Models
{
	public record ReviewWordRequest(int WordId, int UserId, int DictionaryId, ReviewGrade Grade);
}

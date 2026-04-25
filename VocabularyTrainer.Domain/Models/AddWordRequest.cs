namespace VocabularyTrainer.Domain.Models
{
	public record AddWordRequest
	(
		string Value,
		string Translation,
		int UserId,
		int DictionaryId
	);
}

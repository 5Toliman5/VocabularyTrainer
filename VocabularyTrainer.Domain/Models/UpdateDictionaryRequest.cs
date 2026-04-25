namespace VocabularyTrainer.Domain.Models
{
	public record UpdateDictionaryRequest
	(
		int DictionaryId,
		int UserId,
		string Name,
		string? LanguageCode
	);
}

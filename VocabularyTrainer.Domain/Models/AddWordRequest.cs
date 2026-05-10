namespace VocabularyTrainer.Domain.Models
{
	public record AddWordRequest
	(
		string Value,
		IReadOnlyList<WordTranslationDto> Translations,
		int UserId,
		int DictionaryId,
		string? Notes = null
	);
}

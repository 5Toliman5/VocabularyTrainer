namespace VocabularyTrainer.Domain.Models
{
	public record UpdateDictionaryRequest(int DictionaryId, string Name, string? LanguageCode);
}

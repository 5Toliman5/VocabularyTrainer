namespace VocabularyTrainer.Domain.Models
{
	public record WordTranslationDto(string Text, TranslationKind Kind = TranslationKind.Translation);
}

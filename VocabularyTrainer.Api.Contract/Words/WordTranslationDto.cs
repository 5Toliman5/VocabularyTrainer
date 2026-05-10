namespace VocabularyTrainer.Api.Contract.Words
{
    public record WordTranslationDto(string Text, TranslationKind Kind = TranslationKind.Translation);
}

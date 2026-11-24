namespace VocabularyTrainer.Api.Contract.Words
{
    public record AddWordRequest
    (
        int DictionaryId,
        string Value,
        IReadOnlyList<WordTranslationDto> Translations
    );
}

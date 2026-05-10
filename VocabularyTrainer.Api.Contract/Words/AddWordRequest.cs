namespace VocabularyTrainer.Api.Contract.Words
{
    public record AddWordRequest
    (
        int UserId,
        int DictionaryId,
        string Value,
        IReadOnlyList<WordTranslationDto> Translations,
        string? Notes = null
    );
}

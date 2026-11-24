namespace VocabularyTrainer.Api.Contract.Words
{
    public record WordResponse
    (
        int Id,
        int DictionaryId,
        string DictionaryName,
        string Value,
        string LanguageCode,
        IReadOnlyList<WordTranslationDto> Translations,
        DateTime DateAdded,
        DateTime DateModified
    );
}

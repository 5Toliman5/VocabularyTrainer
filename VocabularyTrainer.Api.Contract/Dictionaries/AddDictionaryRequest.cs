namespace VocabularyTrainer.Api.Contract.Dictionaries
{
    public record AddDictionaryRequest
    (
        int UserId,
        string Name,
        string? LanguageCode
    );
}

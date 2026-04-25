namespace VocabularyTrainer.Api.Contract.Dictionaries
{
    public record UpdateDictionaryRequest
    (
        int UserId,
        string Name,
        string? LanguageCode
    );
}

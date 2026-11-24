namespace VocabularyTrainer.Api.Contract.Dictionaries
{
    public record UpdateDictionaryRequest
    (
        string Name,
        string? LanguageCode,
        string AlgorithmCode
    );
}

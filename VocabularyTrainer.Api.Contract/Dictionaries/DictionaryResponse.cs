namespace VocabularyTrainer.Api.Contract.Dictionaries
{
    public record DictionaryResponse
    (
        int Id,
        string Name,
        string? LanguageCode,
        string AlgorithmCode,
        int WordCount
    );
}

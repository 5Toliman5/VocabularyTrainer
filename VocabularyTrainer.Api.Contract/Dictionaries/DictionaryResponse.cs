namespace VocabularyTrainer.Api.Contract.Dictionaries
{
    /// <summary>Represents a user dictionary with its word count.</summary>
    public record DictionaryResponse(int Id, string Name, string? LanguageCode, int WordCount);
}

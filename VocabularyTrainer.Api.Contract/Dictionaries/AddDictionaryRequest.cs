namespace VocabularyTrainer.Api.Contract.Dictionaries
{
    /// <summary>Request body for creating a new user dictionary.</summary>
    public record AddDictionaryRequest(int UserId, string Name, string? LanguageCode);
}

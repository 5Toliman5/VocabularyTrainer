namespace VocabularyTrainer.Api.Contract.Dictionaries
{
    /// <summary>Request body for renaming or updating a user dictionary. The dictionary ID is supplied via the route.</summary>
    public record UpdateDictionaryRequest(int UserId, string Name, string? LanguageCode);
}

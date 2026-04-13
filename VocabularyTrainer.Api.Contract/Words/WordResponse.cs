namespace VocabularyTrainer.Api.Contract.Words
{
    /// <summary>Represents a word with its training weight and dictionary context.</summary>
    public record WordResponse(int Id, string Value, string Translation, int Weight, int DictionaryId, string DictionaryName);
}

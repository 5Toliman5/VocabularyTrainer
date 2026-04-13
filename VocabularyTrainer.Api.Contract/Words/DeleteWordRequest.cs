namespace VocabularyTrainer.Api.Contract.Words
{
    /// <summary>Request body for deleting a word identified by its composite key.</summary>
    public record DeleteWordRequest(int WordId, int UserId, int DictionaryId);
}

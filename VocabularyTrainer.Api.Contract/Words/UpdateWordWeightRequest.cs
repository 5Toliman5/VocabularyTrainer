namespace VocabularyTrainer.Api.Contract.Words
{
    /// <summary>Request body for updating the training weight of a word.</summary>
    public record UpdateWordWeightRequest(int WordId, int UserId, int DictionaryId, WeightUpdateType Type);
}

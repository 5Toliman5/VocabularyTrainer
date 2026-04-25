namespace VocabularyTrainer.Api.Contract.Words
{
    public record UpdateWordWeightRequest
    (
        int WordId,
        int UserId,
        int DictionaryId,
        WeightUpdateType Type
    );
}

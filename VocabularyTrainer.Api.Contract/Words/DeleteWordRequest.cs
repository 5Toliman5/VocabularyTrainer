namespace VocabularyTrainer.Api.Contract.Words
{
    public record DeleteWordRequest
    (
        int WordId,
        int UserId,
        int DictionaryId
    );
}

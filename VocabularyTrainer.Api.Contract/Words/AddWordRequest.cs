namespace VocabularyTrainer.Api.Contract.Words
{
    public record AddWordRequest
    (
        string Value,
        string Translation,
        int UserId,
        int DictionaryId
    );
}

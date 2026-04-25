namespace VocabularyTrainer.Api.Contract.Words
{
    public record WordResponse
    (
        int Id,
        string Value,
        string Translation,
        int Weight,
        int DictionaryId,
        string DictionaryName
    );
}

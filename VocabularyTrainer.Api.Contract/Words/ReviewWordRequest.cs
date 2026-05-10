namespace VocabularyTrainer.Api.Contract.Words
{
    public record ReviewWordRequest(int UserId, int DictionaryId, ReviewGrade Grade);
}

namespace VocabularyTrainer.Domain.Exceptions
{
	public class DatabaseException(string message, Exception innerException) : Exception(message, innerException)
	{
    }
}

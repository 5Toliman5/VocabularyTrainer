namespace VocabularyTrainer.Domain.Exceptions
{
	/// <summary>Exception thrown when a database operation fails.</summary>
	public class DatabaseException(string message, Exception innerException) : Exception(message, innerException)
	{
	}
}

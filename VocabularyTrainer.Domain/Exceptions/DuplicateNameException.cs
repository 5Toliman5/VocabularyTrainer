namespace VocabularyTrainer.Domain.Exceptions
{
	/// <summary>Exception thrown when an insert or update violates a unique-name constraint.</summary>
	public class DuplicateNameException(Exception innerException) : DatabaseException("Duplicate name constraint violation.", innerException)
	{
	}
}

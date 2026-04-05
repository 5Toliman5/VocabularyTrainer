namespace VocabularyTrainer.Domain.Exceptions
{
	public class DuplicateNameException(Exception innerException) : DatabaseException("Duplicate name constraint violation.", innerException)
	{
    }
}

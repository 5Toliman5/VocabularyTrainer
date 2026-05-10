namespace VocabularyTrainer.Domain.Exceptions
{
	public sealed class DuplicateKeyException(string message, Exception? innerException = null) : Exception(message, innerException)
	{
	}
}

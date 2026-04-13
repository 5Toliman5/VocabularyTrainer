namespace VocabularyTrainer.Domain.Models
{
	/// <summary>Domain-level constants for word training rules.</summary>
	public static class WordConstants
	{
		/// <summary>Maximum training weight a word can accumulate before it stops being promoted.</summary>
		public const int MaxWeight = 10;
	}
}

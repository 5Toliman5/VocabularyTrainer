namespace VocabularyTrainer.Domain.Models
{
	public static class WordConstants
	{
		// Default placeholder used when a dictionary has no LanguageCode set.
		public const string UnknownLanguage = "unknown";

		// Default size of the in-memory training session candidate batch
		// pulled from the API. Algorithms may request a larger pool internally.
		public const int DefaultCandidateBatchSize = 25;
	}
}

namespace VocabularyTrainer.Domain.Models
{
	public record AddDictionaryRequest
	(
		int UserId,
		string Name,
		string? LanguageCode,
		string AlgorithmCode = AlgorithmCodes.Default
	);
}

namespace VocabularyTrainer.Domain.Models
{
	public record AlgorithmInfo
	(
		string Code,
		string DisplayName,
		string Description,
		AlgorithmCost Cost,
		IReadOnlyList<ReviewGrade> SupportedGrades
	);
}

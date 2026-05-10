using VocabularyTrainer.Api.Contract.Words;

namespace VocabularyTrainer.Api.Contract.Algorithms
{
    public record AlgorithmInfoResponse
    (
        string Code,
        string DisplayName,
        string Description,
        AlgorithmCost Cost,
        IReadOnlyList<ReviewGrade> SupportedGrades
    );
}

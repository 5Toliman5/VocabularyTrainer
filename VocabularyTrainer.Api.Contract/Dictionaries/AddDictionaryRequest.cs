namespace VocabularyTrainer.Api.Contract.Dictionaries
{
    // The default mirrors VocabularyTrainer.Domain.Models.AlgorithmCodes.Default.
    // Kept as a literal here to keep Api.Contract free of Domain references.
    public record AddDictionaryRequest
    (
        int UserId,
        string Name,
        string? LanguageCode,
        string AlgorithmCode = "WeightBased"
    );
}

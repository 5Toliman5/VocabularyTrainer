using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface ITrainingAlgorithmRegistry
	{
		IReadOnlyList<IWordTrainingAlgorithm> All { get; }

		IReadOnlyList<AlgorithmInfo> AllInfos { get; }

		IWordTrainingAlgorithm GetByCode(string code);

		bool IsKnown(string code);
	}
}

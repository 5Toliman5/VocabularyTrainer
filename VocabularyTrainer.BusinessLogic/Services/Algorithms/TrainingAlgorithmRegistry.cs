using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Algorithms
{
	public class TrainingAlgorithmRegistry : ITrainingAlgorithmRegistry
	{
		private readonly IReadOnlyDictionary<string, IWordTrainingAlgorithm> _byCode;

		public IReadOnlyList<IWordTrainingAlgorithm> All { get; }
		public IReadOnlyList<AlgorithmInfo> AllInfos { get; }

		public TrainingAlgorithmRegistry(IEnumerable<IWordTrainingAlgorithm> algorithms)
		{
			All = algorithms
				.OrderBy(a => a.Info.Cost)
				.ThenBy(a => a.Info.DisplayName, StringComparer.Ordinal)
				.ToList();

			_byCode = All.ToDictionary(a => a.Info.Code, StringComparer.OrdinalIgnoreCase);
			AllInfos = All.Select(a => a.Info).ToList();
		}

		public IWordTrainingAlgorithm GetByCode(string code)
		{
			if (_byCode.TryGetValue(code, out var algo))
			{
				return algo;
			}

			throw new InvalidOperationException($"No training algorithm registered for code '{code}'.");
		}

		public bool IsKnown(string code) => _byCode.ContainsKey(code);
	}
}

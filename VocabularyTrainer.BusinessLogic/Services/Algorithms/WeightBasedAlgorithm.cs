using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Algorithms
{
	public class WeightBasedAlgorithm(
		IWordWeightBasedParamsRepository weightRepository,
		IWordRepository wordRepository) : IWordTrainingAlgorithm
	{
		public const string Code = AlgorithmCodes.WeightBased;
		public const int MaxWeight = 10;
		public const int NewCardWeight = MaxWeight / 2;

		private const int MinWordWeight = 0;
		private const double MinExclusiveUniformSample = 1e-12;

		public AlgorithmInfo Info { get; } = new(
			Code,
			"Weight-based",
			"Each word carries an integer weight; words with higher weight are shown more often. " +
			"Weight goes up on incorrect answers and down on correct ones.",
			AlgorithmCost.Free,
			[ReviewGrade.Again, ReviewGrade.Good]);

		public async Task<List<WordDto>> GetTrainingCandidatesAsync(int userId, int? dictionaryId, int limit)
		{
			var pool = await weightRepository.GetCandidatePoolAsync(userId, dictionaryId);

			if (pool.Count == 0)
			{
				return [];
			}

			var selectedWordIds = pool
				.Select(poolEntry =>
				{
					var samplingWeight = (poolEntry.Weight ?? NewCardWeight) + 1;
					var reservoirKey = -Math.Log(SafeUniform()) / samplingWeight;
					return (poolEntry.WordId, ReservoirKey: reservoirKey);
				})
				.OrderBy(sample => sample.ReservoirKey)
				.Take(limit)
				.Select(sample => sample.WordId)
				.ToList();

			var loadedWords = await wordRepository.GetByIdsAsync(userId, selectedWordIds);
			var wordsById = loadedWords.ToDictionary(word => word.Id);

			return selectedWordIds
				.Where(wordsById.ContainsKey)
				.Select(wordId => wordsById[wordId])
				.ToList();
		}

		public async Task ApplyReviewAsync(WordDto card, ReviewGrade grade)
		{
			var current = (await weightRepository.GetAsync(card.Id))?.Weight ?? 0;
			var updated = grade switch
			{
				ReviewGrade.Again => Math.Min(MaxWeight, current + 1),
				ReviewGrade.Hard => Math.Min(MaxWeight, current + 1),
				ReviewGrade.Good => Math.Max(MinWordWeight, current - 1),
				ReviewGrade.Easy => Math.Max(MinWordWeight, current - 2),
				_ => current,
			};

			await weightRepository.UpsertAsync(card.Id, updated);
		}

		// NextDouble() can be 0; -log(0) is undefined.
		private static double SafeUniform()
		{
			var uniformSample = Random.Shared.NextDouble();
			return uniformSample <= 0 ? MinExclusiveUniformSample : uniformSample;
		}
	}
}

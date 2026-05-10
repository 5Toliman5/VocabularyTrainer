using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Algorithms
{
	public class Sm2Algorithm(IWordSm2ParamsRepository sm2Repository, IWordRepository wordRepository) : IWordTrainingAlgorithm
	{
		public const string Code = AlgorithmCodes.Sm2;
		public const decimal MinEaseFactor = 1.30m;
		public const decimal DefaultEaseFactor = 2.50m;

		private const int ResponseQualityAgain = 0;
		private const int ResponseQualityHard = 3;
		private const int ResponseQualityGood = 4;
		private const int ResponseQualityEasy = 5;
		private const int MaxResponseQuality = 5;
		private const int MinimumSuccessfulResponseQuality = 3;
		private const int FirstRepetitionIntervalDays = 1;
		private const int SecondRepetitionIntervalDays = 6;
		private const int MinimumScheduledIntervalDays = 1;
		private const decimal EaseAdjustmentBase = 0.1m;
		private const decimal EaseAdjustmentLinearMultiplier = 0.08m;
		private const decimal EaseAdjustmentQuadraticMultiplier = 0.02m;

		public AlgorithmInfo Info { get; } = new(
			Code,
			"SM-2 (spaced repetition)",
			"Classic SuperMemo-2 algorithm: schedules each word at expanding intervals based on " +
			"how easy you found it. Cards become due after their interval and surface in the next session.",
			AlgorithmCost.Free,
			[ReviewGrade.Again, ReviewGrade.Hard, ReviewGrade.Good, ReviewGrade.Easy]);

		public async Task<List<WordDto>> GetTrainingCandidatesAsync(int userId, int? dictionaryId, int limit)
		{
			var ids = await sm2Repository.GetCandidateIdsAsync(userId, dictionaryId, limit);

			if (ids.Count == 0)
			{
				return [];
			}

			var loadedWords = await wordRepository.GetByIdsAsync(userId, ids);
			var wordsById = loadedWords.ToDictionary(word => word.Id);

			return ids.Where(wordsById.ContainsKey).Select(wordId => wordsById[wordId]).ToList();
		}

		public async Task ApplyReviewAsync(WordDto card, ReviewGrade grade)
		{
			var responseQuality = MapToResponseQuality(grade);
			var priorParams = await sm2Repository.GetAsync(card.Id);
			var nowUtc = DateTime.UtcNow;

			var easeFactor = priorParams?.EaseFactor ?? DefaultEaseFactor;
			var repetitions = priorParams?.Repetitions ?? 0;
			var intervalDays = priorParams?.IntervalDays ?? 0;
			var lapses = priorParams?.Lapses ?? 0;

			// Decimal SM-2 update avoids float drift.
			var qualityDelta = MaxResponseQuality - responseQuality;
			easeFactor = Math.Max(
				MinEaseFactor,
				easeFactor
					+ (EaseAdjustmentBase
						- qualityDelta * (EaseAdjustmentLinearMultiplier + qualityDelta * EaseAdjustmentQuadraticMultiplier)));

			if (responseQuality < MinimumSuccessfulResponseQuality)
			{
				lapses += 1;
				repetitions = 0;
				intervalDays = FirstRepetitionIntervalDays;
			}
			else
			{
				intervalDays = repetitions switch
				{
					0 => FirstRepetitionIntervalDays,
					1 => SecondRepetitionIntervalDays,
					_ => Math.Max(
						MinimumScheduledIntervalDays,
						(int)Math.Round(intervalDays * easeFactor, MidpointRounding.AwayFromZero)),
				};
				repetitions += 1;
			}

			await sm2Repository.UpsertAsync(new WordSm2Params
			{
				WordId = card.Id,
				Repetitions = repetitions,
				IntervalDays = intervalDays,
				EaseFactor = easeFactor,
				Lapses = lapses,
				LastReviewedAt = nowUtc,
				NextDueAt = nowUtc.AddDays(intervalDays),
			});
		}

		private static int MapToResponseQuality(ReviewGrade grade) => grade switch
		{
			ReviewGrade.Again => ResponseQualityAgain,
			ReviewGrade.Hard => ResponseQualityHard,
			ReviewGrade.Good => ResponseQualityGood,
			ReviewGrade.Easy => ResponseQualityEasy,
			_ => throw new ArgumentOutOfRangeException(nameof(grade), grade, "Unknown ReviewGrade for SM-2."),
		};
	}
}

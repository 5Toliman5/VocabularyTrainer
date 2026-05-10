using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class TrainingService(ITrainingAlgorithmRegistry registry, IDictionaryRepository dictionaries)
		: ITrainingService
	{
		public async Task<Result<List<WordDto>>> GetCandidatesAsync(int userId, DictionaryScope scope, int limit)
		{
			if (limit <= 0)
			{
				return Result<List<WordDto>>.Success([]);
			}

			if (scope.IsAll)
			{
				return Result<List<WordDto>>.Success(await GetCandidatesAcrossAllDictionariesAsync(userId, limit));
			}

			return await GetCandidatesForSingleDictionaryAsync(userId, scope.DictionaryId!.Value, limit);
		}

		private async Task<Result<List<WordDto>>> GetCandidatesForSingleDictionaryAsync(int userId, int dictionaryId, int limit)
		{
			var algorithmResult = await ResolveAlgorithmAsync(dictionaryId, userId);

			if (!algorithmResult.Successful)
			{
				return Result<List<WordDto>>.Failure(algorithmResult);
			}

			var cards = await algorithmResult.Value.GetTrainingCandidatesAsync(userId, dictionaryId, limit);
			return Result<List<WordDto>>.Success(cards);
		}

		private async Task<List<WordDto>> GetCandidatesAcrossAllDictionariesAsync(int userId, int limit)
		{
			var userDictionaries = await dictionaries.GetAllAsync(userId);

			if (userDictionaries.Count == 0)
			{
				return [];
			}

			var candidateBatchTasks = userDictionaries
				.Where(dictionary => registry.IsKnown(dictionary.AlgorithmCode))
				.Select(dictionary => registry.GetByCode(dictionary.AlgorithmCode)
					.GetTrainingCandidatesAsync(userId, dictionary.Id, limit))
				.ToList();

			var candidateBatches = await Task.WhenAll(candidateBatchTasks);

			var merged = new List<WordDto>(capacity: Math.Min(limit, candidateBatches.Sum(batch => batch.Count)));
			var batchEnumerators = candidateBatches.Select(batch => batch.GetEnumerator()).ToArray();

			try
			{
				bool anyBatchAdvanced;

				do
				{
					anyBatchAdvanced = false;

					for (var dictionaryIndex = 0;
						dictionaryIndex < batchEnumerators.Length && merged.Count < limit;
						dictionaryIndex++)
					{
						var batchEnumerator = batchEnumerators[dictionaryIndex];

						if (!batchEnumerator.MoveNext())
						{
							continue;
						}

						merged.Add(batchEnumerator.Current);
						anyBatchAdvanced = true;
					}
				}
				while (anyBatchAdvanced && merged.Count < limit);
			}
			finally
			{
				foreach (var batchEnumerator in batchEnumerators)
				{
					batchEnumerator.Dispose();
				}
			}

			return merged;
		}

		public async Task<Result> ApplyReviewAsync(ReviewWordRequest request)
		{
			var algorithmResult = await ResolveAlgorithmAsync(request.DictionaryId, request.UserId);

			if (!algorithmResult.Successful)
			{
				return Result.Failure(algorithmResult);
			}

			var card = new WordDto
			{
				Id = request.WordId,
				UserId = request.UserId,
				DictionaryId = request.DictionaryId,
			};

			await algorithmResult.Value.ApplyReviewAsync(card, request.Grade);
			return Result.Success();
		}

		public async Task<Result<IReadOnlyList<ReviewGrade>>> GetSupportedGradesAsync(int dictionaryId, int userId)
		{
			var algorithmResult = await ResolveAlgorithmAsync(dictionaryId, userId);

			if (!algorithmResult.Successful)
			{
				return Result<IReadOnlyList<ReviewGrade>>.Failure(algorithmResult);
			}

			return Result<IReadOnlyList<ReviewGrade>>.Success(algorithmResult.Value.Info.SupportedGrades);
		}

		private async Task<Result<IWordTrainingAlgorithm>> ResolveAlgorithmAsync(int dictionaryId, int userId)
		{
			var dictionary = await dictionaries.GetByIdAsync(dictionaryId, userId);

			if (dictionary is null)
			{
				return Result<IWordTrainingAlgorithm>.Failure(
					$"Dictionary {dictionaryId} was not found for user {userId}.", ResultErrorKind.NotFound);
			}

			if (!registry.IsKnown(dictionary.AlgorithmCode))
			{
				return Result<IWordTrainingAlgorithm>.Failure(
					$"Unknown training algorithm '{dictionary.AlgorithmCode}'.", ResultErrorKind.Validation);
			}

			return Result<IWordTrainingAlgorithm>.Success(registry.GetByCode(dictionary.AlgorithmCode));
		}
	}
}

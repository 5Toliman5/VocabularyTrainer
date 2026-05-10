using Common.Wrappers;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class WordService(
		IWordRepository wordRepository,
		IDictionaryRepository? dictionaryRepository = null) : IWordService
	{
		public Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
			=> wordRepository.GetAllAsync(userId, dictionaryId);

		public Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request)
			=> wordRepository.GetPagedAsync(request);

		public async Task<Result<int>> AddAsync(AddWordRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Value))
			{
				return Result<int>.Failure("Word value must not be empty.", ResultErrorKind.Validation);
			}

			if (request.Translations.Count == 0 || request.Translations.All(t => string.IsNullOrWhiteSpace(t.Text)))
			{
				return Result<int>.Failure("At least one translation is required.", ResultErrorKind.Validation);
			}

			try
			{
				int wordId;
				if (dictionaryRepository is null)
				{
					wordId = await wordRepository.AddAsync(request, normalizedText: string.Empty, languageCode: string.Empty);
				}
				else
				{
					var dictionary = await dictionaryRepository.GetByIdAsync(request.DictionaryId, request.UserId);

					if (dictionary is null)
					{
						return Result<int>.Failure(
							$"Dictionary {request.DictionaryId} was not found for user {request.UserId}.",
							ResultErrorKind.NotFound);
					}

					var normalized = WordTextNormalizer.Normalize(request.Value);
					var languageCode = ResolveLanguageCode(dictionary);
					wordId = await wordRepository.AddAsync(request, normalized, languageCode);
				}

				return Result<int>.Success(wordId);
			}
			catch (DuplicateKeyException ex)
			{
				return Result<int>.Failure(ex.Message, ResultErrorKind.Conflict);
			}
			catch (EntityNotFoundException ex)
			{
				return Result<int>.Failure(ex.Message, ResultErrorKind.NotFound);
			}
			catch (DomainValidationException ex)
			{
				return Result<int>.Failure(ex.Message, ResultErrorKind.Validation);
			}
		}

		public async Task<Result> DeleteAsync(int wordId, int userId)
		{
			try
			{
				var rowsDeleted = await wordRepository.DeleteAsync(wordId, userId);
				if (rowsDeleted == 0)
				{
					return Result.Failure($"Word {wordId} was not found.", ResultErrorKind.NotFound);
				}

				return Result.Success();
			}
			catch (EntityNotFoundException ex)
			{
				return Result.Failure(ex.Message, ResultErrorKind.NotFound);
			}
		}

		private static string ResolveLanguageCode(DictionaryDto dictionary) =>
			string.IsNullOrWhiteSpace(dictionary.LanguageCode)
				? WordConstants.UnknownLanguage
				: dictionary.LanguageCode!;
	}
}

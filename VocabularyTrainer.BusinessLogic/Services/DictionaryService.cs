using Common.Wrappers;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class DictionaryService(
		IDictionaryRepository dictionaryRepository,
		ITrainingAlgorithmRegistry? algorithmRegistry = null,
		IAlgorithmRepository? algorithmRepository = null) : IDictionaryService
	{
		public Task<List<DictionaryDto>> GetAllAsync(int userId)
			=> dictionaryRepository.GetAllAsync(userId);

		public Task<DictionaryDto?> GetByIdAsync(int dictionaryId, int userId)
			=> dictionaryRepository.GetByIdAsync(dictionaryId, userId);

		public async Task<Result<DictionaryDto>> AddAsync(AddDictionaryRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Name))
			{
				return Result<DictionaryDto>.Failure("Dictionary name must not be empty.", ResultErrorKind.Validation);
			}

			var algorithmResolution = await ResolveAlgorithmPersistenceIdAsync(request.AlgorithmCode);
			if (!algorithmResolution.Ok)
			{
				return Result<DictionaryDto>.Failure(
					$"Unknown training algorithm '{request.AlgorithmCode}'.", ResultErrorKind.Validation);
			}

			try
			{
				var dictionaryId = await dictionaryRepository.AddAsync(request, algorithmResolution.PersistenceId);
				return Result<DictionaryDto>.Success(new DictionaryDto(
					dictionaryId, request.Name, request.LanguageCode, request.AlgorithmCode));
			}
			catch (DuplicateKeyException ex)
			{
				return Result<DictionaryDto>.Failure(ex.Message, ResultErrorKind.Conflict);
			}
			catch (DomainValidationException ex)
			{
				return Result<DictionaryDto>.Failure(ex.Message, ResultErrorKind.Validation);
			}
			catch (EntityNotFoundException ex)
			{
				return Result<DictionaryDto>.Failure(ex.Message, ResultErrorKind.NotFound);
			}
		}

		public async Task<Result> UpdateAsync(UpdateDictionaryRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Name))
			{
				return Result.Failure("Dictionary name must not be empty.", ResultErrorKind.Validation);
			}

			var algorithmResolution = await ResolveAlgorithmPersistenceIdAsync(request.AlgorithmCode);
			if (!algorithmResolution.Ok)
			{
				return Result.Failure(
					$"Unknown training algorithm '{request.AlgorithmCode}'.", ResultErrorKind.Validation);
			}

			try
			{
				var rowsUpdated = await dictionaryRepository.UpdateAsync(request, algorithmResolution.PersistenceId);
				if (rowsUpdated == 0)
				{
					return Result.Failure(
						$"Dictionary {request.DictionaryId} was not found for user {request.UserId}.",
						ResultErrorKind.NotFound);
				}

				return Result.Success();
			}
			catch (DuplicateKeyException ex)
			{
				return Result.Failure(ex.Message, ResultErrorKind.Conflict);
			}
			catch (DomainValidationException ex)
			{
				return Result.Failure(ex.Message, ResultErrorKind.Validation);
			}
			catch (EntityNotFoundException ex)
			{
				return Result.Failure(ex.Message, ResultErrorKind.NotFound);
			}
		}

		public Task DeleteAsync(int dictionaryId, int userId)
			=> dictionaryRepository.DeleteAsync(dictionaryId, userId);

		private async Task<(bool Ok, int PersistenceId)> ResolveAlgorithmPersistenceIdAsync(string algorithmCode)
		{
			if (algorithmRepository is not null)
			{
				var id = await algorithmRepository.GetIdByCodeAsync(algorithmCode);
				if (!id.HasValue)
				{
					return (false, 0);
				}

				return (true, id.Value);
			}

			if (algorithmRegistry is not null && !algorithmRegistry.IsKnown(algorithmCode))
			{
				return (false, 0);
			}

			return (true, 0);
		}
	}
}

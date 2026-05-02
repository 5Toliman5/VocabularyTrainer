using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class DictionaryService(IDictionaryRepository repository) : IDictionaryService
	{
		public Task<List<DictionaryDto>> GetAllAsync(int userId)
		{
			return repository.GetAllAsync(userId);
		}

		public async Task<Result<DictionaryDto>> AddAsync(AddDictionaryRequest request)
		{
			var result = await repository.AddAsync(request);
			if (!result.Successful)
				return Result<DictionaryDto>.Failure(result.ErrorMessage!);

			return Result<DictionaryDto>.Success(new DictionaryDto(result.Value, request.Name, request.LanguageCode));
		}

		public Task<Result> UpdateAsync(UpdateDictionaryRequest request)
		{
			return repository.UpdateAsync(request);
		}

		public Task DeleteAsync(int dictionaryId, int userId)
		{
			return repository.DeleteAsync(dictionaryId, userId);
		}
	}
}

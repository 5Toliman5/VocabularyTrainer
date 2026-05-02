using Common.Web.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public class ApiDictionaryService(IDictionaryService dictionaryService) : IApiDictionaryService
    {
        public Task<List<DictionaryDto>> GetAllAsync(int userId)
            => dictionaryService.GetAllAsync(userId);

        public async Task<ApiOperationResult<DictionaryDto>> AddAsync(AddDictionaryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiOperationResult<DictionaryDto>.Failure("Dictionary name is required.", ApiErrorType.Validation);

            var result = await dictionaryService.AddAsync(request);
            if (!result.Successful)
                return ApiOperationResult<DictionaryDto>.Failure(result.ErrorMessage!, ApiErrorType.Conflict);

            return ApiOperationResult<DictionaryDto>.Success(result.Value);
        }

        public async Task<ApiOperationResult> UpdateAsync(UpdateDictionaryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiOperationResult.Failure("Dictionary name is required.", ApiErrorType.Validation);

            var result = await dictionaryService.UpdateAsync(request);
            if (!result.Successful)
                return ApiOperationResult.Failure(result.ErrorMessage!, ApiErrorType.Conflict);

            return ApiOperationResult.Success();
        }

        public Task DeleteAsync(int dictionaryId, int userId)
            => dictionaryService.DeleteAsync(dictionaryId, userId);
    }
}

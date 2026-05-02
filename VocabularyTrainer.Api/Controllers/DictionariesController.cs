using AutoMapper;
using Common.Web.Controllers;
using Common.Web.Wrappers;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Contract.Dictionaries;
using VocabularyTrainer.Domain.Repositories;
using DomainAddDictRequest = VocabularyTrainer.Domain.Models.AddDictionaryRequest;
using DomainUpdateDictRequest = VocabularyTrainer.Domain.Models.UpdateDictionaryRequest;

namespace VocabularyTrainer.Api.Controllers
{
    [Route("api/[controller]")]
    public class DictionariesController(IDictionaryRepository repository, IMapper mapper) : BaseApiController
    {
        [HttpGet]
        public async Task<IEnumerable<DictionaryResponse>> GetAll([FromQuery] int userId)
        {
            var dictionaries = await repository.GetAllAsync(userId);
            return mapper.Map<IEnumerable<DictionaryResponse>>(dictionaries);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDictionaryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResolveFailure(ApiResult.Failure("Dictionary name is required.", ApiErrorType.Validation));

            var result = await repository.AddAsync(mapper.Map<DomainAddDictRequest>(request));
            if (!result.Successful)
                return ResolveFailure(ApiResult.Failure(result.ErrorMessage!, ApiErrorType.Conflict));

            var response = new DictionaryResponse(result.Value, request.Name, request.LanguageCode, 0);
            return CreatedAtAction(nameof(GetAll), new { userId = request.UserId }, response);
        }

        [HttpPut("{dictionaryId:int}")]
        public async Task<IActionResult> Update(int dictionaryId, [FromBody] UpdateDictionaryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResolveFailure(ApiResult.Failure("Dictionary name is required.", ApiErrorType.Validation));

            var domainRequest = mapper.Map<DomainUpdateDictRequest>(
                request, opts => opts.Items["dictionaryId"] = dictionaryId);
            var result = await repository.UpdateAsync(domainRequest);
            if (!result.Successful)
                return ResolveFailure(ApiResult.Failure(result.ErrorMessage!, ApiErrorType.Conflict));

            return NoContent();
        }

        [HttpDelete("{dictionaryId:int}")]
        public async Task<IActionResult> Delete(int dictionaryId, [FromQuery] int userId)
        {
            await repository.DeleteAsync(dictionaryId, userId);
            return NoContent();
        }
    }
}

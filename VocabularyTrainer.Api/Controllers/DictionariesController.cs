using AutoMapper;
using Common.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Contract.Dictionaries;
using VocabularyTrainer.Api.BusinessLogic.Services;
using DomainAddDictRequest = VocabularyTrainer.Domain.Models.AddDictionaryRequest;
using DomainUpdateDictRequest = VocabularyTrainer.Domain.Models.UpdateDictionaryRequest;

namespace VocabularyTrainer.Api.Controllers
{
    [Route("api/[controller]")]
    public class DictionariesController(IApiDictionaryService service, IMapper mapper) : BaseApiController
    {
        [HttpGet]
        public async Task<IEnumerable<DictionaryResponse>> GetAll([FromQuery] int userId)
        {
            var dictionaries = await service.GetAllAsync(userId);
            return mapper.Map<IEnumerable<DictionaryResponse>>(dictionaries);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDictionaryRequest request)
        {
            var result = await service.AddAsync(mapper.Map<DomainAddDictRequest>(request));
            if (!result.Successful)
                return ResolveFailure(result);

            return CreatedAtAction(nameof(GetAll), new { userId = request.UserId }, mapper.Map<DictionaryResponse>(result.Value));
        }

        [HttpPut("{dictionaryId:int}")]
        public async Task<IActionResult> Update(int dictionaryId, [FromBody] UpdateDictionaryRequest request)
        {
            var domainRequest = mapper.Map<DomainUpdateDictRequest>(
                request, opts => opts.Items["dictionaryId"] = dictionaryId);
            var result = await service.UpdateAsync(domainRequest);
            if (!result.Successful)
                return ResolveFailure(result);

            return NoContent();
        }

        [HttpDelete("{dictionaryId:int}")]
        public async Task<IActionResult> Delete(int dictionaryId, [FromQuery] int userId)
        {
            await service.DeleteAsync(dictionaryId, userId);
            return NoContent();
        }
    }
}

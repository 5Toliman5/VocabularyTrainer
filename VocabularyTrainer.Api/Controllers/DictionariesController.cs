using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Api.Contract.Dictionaries;
using VocabularyTrainer.Api.Infrastructure;
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

        [HttpGet("{dictionaryId:int}", Name = "GetDictionaryById")]
        public async Task<IActionResult> GetById(int dictionaryId, [FromQuery] int userId)
        {
            var dict = await service.GetByIdAsync(dictionaryId, userId);
            return dict is null
                ? NotFound()
                : Ok(mapper.Map<DictionaryResponse>(dict));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDictionaryRequest request)
        {
            var result = await service.AddAsync(mapper.Map<DomainAddDictRequest>(request));
            if (!result.Successful)
                return ResolveFailure(result);

            var response = mapper.Map<DictionaryResponse>(result.Value);
            return CreatedAtRoute("GetDictionaryById",
                new { dictionaryId = response.Id, userId = request.UserId },
                response);
        }

        [HttpPut("{dictionaryId:int}")]
        public async Task<IActionResult> Update(int dictionaryId, [FromBody] UpdateDictionaryRequest request)
        {
            var domainRequest = new DomainUpdateDictRequest(
                dictionaryId, request.UserId, request.Name, request.LanguageCode, request.AlgorithmCode);

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

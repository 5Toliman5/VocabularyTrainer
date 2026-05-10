using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Infrastructure;
using VocabularyTrainer.Domain.Models;
using AddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using WordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using WordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
using GetWordsPagedReq = VocabularyTrainer.Api.Contract.Words.GetWordsPagedRequest;
using DomainAddWordRequest = VocabularyTrainer.Domain.Models.AddWordRequest;
using DomainGetWordsPagedRequest = VocabularyTrainer.Domain.Models.GetWordsPagedRequest;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;

namespace VocabularyTrainer.Api.Controllers
{
    [Route("api/[controller]")]
    public class WordsController(IApiWordService service, IMapper mapper) : BaseApiController
    {
        [HttpGet]
        public async Task<IEnumerable<WordResponse>> GetAll([FromQuery] int userId, [FromQuery] int? dictionaryId = null)
        {
            var words = await service.GetAllAsync(userId, dictionaryId);
            return mapper.Map<IEnumerable<WordResponse>>(words);
        }

        [HttpGet("paged")]
        public async Task<PagedResult<WordPageItem>> GetPaged([FromQuery] GetWordsPagedReq request)
        {
            var domainRequest = mapper.Map<DomainGetWordsPagedRequest>(request);
            var result = await service.GetPagedAsync(domainRequest);
            var items = mapper.Map<IReadOnlyList<WordPageItem>>(result.Items);
            return new PagedResult<WordPageItem>(items, result.TotalCount, result.Page, result.PageSize);
        }

        [HttpGet("{wordId:int}", Name = "GetWordById")]
        public async Task<IActionResult> GetById(int wordId, [FromQuery] int userId)
        {
            var matches = await service.GetAllAsync(userId);
            var match = matches.FirstOrDefault(w => w.Id == wordId);
            return match is null
                ? NotFound()
                : Ok(mapper.Map<WordResponse>(match));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddWordRequest request)
        {
            var result = await service.AddAsync(mapper.Map<DomainAddWordRequest>(request));
            if (!result.Successful)
                return ResolveFailure(result);

            return CreatedAtRoute("GetWordById",
                new { wordId = result.Value, userId = request.UserId },
                new { id = result.Value });
        }

        [HttpDelete("{wordId:int}")]
        public async Task<IActionResult> Delete(int wordId, [FromQuery] int userId)
        {
            var result = await service.DeleteAsync(wordId, userId);
            if (!result.Successful)
                return ResolveFailure(result);

            return NoContent();
        }
    }
}

using AutoMapper;
using Common.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.BusinessLogic.Services;
using VocabularyTrainer.Domain.Models;
// Contract aliases — take priority over the Domain.Models namespace import for same-named types
using AddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using DeleteWordRequest = VocabularyTrainer.Api.Contract.Words.DeleteWordRequest;
using UpdateWordWeightRequest = VocabularyTrainer.Api.Contract.Words.UpdateWordWeightRequest;
using WordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using WordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
// Domain aliases used inside method bodies when calling the mapper
using DomainAddWordRequest = VocabularyTrainer.Domain.Models.AddWordRequest;
using DomainUserWordKey = VocabularyTrainer.Domain.Models.UserWordKey;
using DomainUpdateWeightRequest = VocabularyTrainer.Domain.Models.UpdateWordWeightRequest;

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
        public async Task<PagedResult<WordPageItem>> GetPaged([FromQuery] GetWordsPagedRequest request)
        {
            var result = await service.GetPagedAsync(request);
            var items = mapper.Map<IReadOnlyList<WordPageItem>>(result.Items);
            return new PagedResult<WordPageItem>(items, result.TotalCount, result.Page, result.PageSize);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddWordRequest request)
        {
            await service.AddAsync(mapper.Map<DomainAddWordRequest>(request));
            return Created();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteWordRequest request)
        {
            await service.DeleteAsync(mapper.Map<DomainUserWordKey>(request));
            return NoContent();
        }

        [HttpPatch("weight")]
        public async Task<IActionResult> UpdateWeight([FromBody] UpdateWordWeightRequest request)
        {
            await service.UpdateWeightAsync(mapper.Map<DomainUpdateWeightRequest>(request));
            return NoContent();
        }
    }
}

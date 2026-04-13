using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Contract.Words;
using VocabularyTrainer.Domain.Repositories;
using DomainAddWordRequest = VocabularyTrainer.Domain.Models.AddWordRequest;
using DomainUserWordKey = VocabularyTrainer.Domain.Models.UserWordKey;
using DomainUpdateWeightRequest = VocabularyTrainer.Domain.Models.UpdateWordWeightRequest;

namespace VocabularyTrainer.Api.Controllers
{
    /// <summary>Manages word entries for a user's dictionary.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WordsController(IWordRepository repository, IMapper mapper) : ControllerBase
    {
        /// <summary>Returns all words for the specified user, optionally filtered by dictionary.</summary>
        [HttpGet]
        public async Task<IEnumerable<WordResponse>> GetAll([FromQuery] int userId, [FromQuery] int? dictionaryId = null)
        {
            var words = await repository.GetAllAsync(userId, dictionaryId);
            return mapper.Map<IEnumerable<WordResponse>>(words);
        }

        /// <summary>Adds a new word to a user's dictionary.</summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddWordRequest request)
        {
            await repository.AddAsync(mapper.Map<DomainAddWordRequest>(request));
            return Created();
        }

        /// <summary>Deletes the word identified by the composite key.</summary>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteWordRequest request)
        {
            await repository.DeleteAsync(mapper.Map<DomainUserWordKey>(request));
            return NoContent();
        }

        /// <summary>Updates the training weight of a word.</summary>
        [HttpPatch("weight")]
        public async Task<IActionResult> UpdateWeight([FromBody] UpdateWordWeightRequest request)
        {
            await repository.UpdateWeightAsync(mapper.Map<DomainUpdateWeightRequest>(request));
            return NoContent();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Contract.Dictionaries;
using VocabularyTrainer.Domain.Repositories;
using DomainAddDictRequest = VocabularyTrainer.Domain.Models.AddDictionaryRequest;
using DomainUpdateDictRequest = VocabularyTrainer.Domain.Models.UpdateDictionaryRequest;

namespace VocabularyTrainer.Api.Controllers
{
    /// <summary>Manages user dictionaries.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DictionariesController(IDictionaryRepository repository, IMapper mapper) : ControllerBase
    {
        /// <summary>Returns all dictionaries belonging to the specified user.</summary>
        [HttpGet]
        public async Task<IEnumerable<DictionaryResponse>> GetAll([FromQuery] int userId)
        {
            var dictionaries = await repository.GetAllAsync(userId);
            return mapper.Map<IEnumerable<DictionaryResponse>>(dictionaries);
        }

        /// <summary>Creates a new dictionary for a user.</summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDictionaryRequest request)
        {
            var id = await repository.AddAsync(mapper.Map<DomainAddDictRequest>(request));
            var response = new DictionaryResponse(id, request.Name, request.LanguageCode, 0);
            return CreatedAtAction(nameof(GetAll), new { userId = request.UserId }, response);
        }

        /// <summary>Updates an existing dictionary's name or language code.</summary>
        [HttpPut("{dictionaryId:int}")]
        public async Task<IActionResult> Update(int dictionaryId, [FromBody] UpdateDictionaryRequest request)
        {
            var domainRequest = mapper.Map<DomainUpdateDictRequest>(
                request, opts => opts.Items["dictionaryId"] = dictionaryId);
            await repository.UpdateAsync(domainRequest);
            return NoContent();
        }

        /// <summary>Deletes a dictionary belonging to the specified user.</summary>
        [HttpDelete("{dictionaryId:int}")]
        public async Task<IActionResult> Delete(int dictionaryId, [FromQuery] int userId)
        {
            await repository.DeleteAsync(dictionaryId, userId);
            return NoContent();
        }
    }
}

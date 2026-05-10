using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Infrastructure;
using VocabularyTrainer.Domain.Models;
using ReviewWordRequest = VocabularyTrainer.Api.Contract.Words.ReviewWordRequest;
using ReviewGrade = VocabularyTrainer.Api.Contract.Words.ReviewGrade;
using WordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using DomainReviewGrade = VocabularyTrainer.Domain.Models.ReviewGrade;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;

namespace VocabularyTrainer.Api.Controllers
{
	[Route("api/training")]
	public class TrainingController(IApiTrainingService service, IMapper mapper) : BaseApiController
	{
		[HttpGet("candidates")]
		public async Task<IActionResult> GetCandidates(
			[FromQuery] int userId,
			[FromQuery] DictionaryScope scope,
			[FromQuery] int limit = WordConstants.DefaultCandidateBatchSize)
		{
			var result = await service.GetCandidatesAsync(userId, scope, limit);
			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(mapper.Map<IEnumerable<WordResponse>>(result.Value));
		}

		[HttpPost("review/{wordId:int}")]
		public async Task<IActionResult> Review(int wordId, [FromBody] ReviewWordRequest request)
		{
			var domainRequest = new VocabularyTrainer.Domain.Models.ReviewWordRequest(
				wordId,
				request.UserId,
				request.DictionaryId,
				mapper.Map<DomainReviewGrade>(request.Grade));

			var result = await service.ApplyReviewAsync(domainRequest);
			if (!result.Successful)
				return ResolveFailure(result);

			return NoContent();
		}

		[HttpGet("supported-grades")]
		public async Task<IActionResult> GetSupportedGrades(
			[FromQuery] int dictionaryId,
			[FromQuery] int userId)
		{
			var result = await service.GetSupportedGradesAsync(dictionaryId, userId);
			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(result.Value.Select(g => mapper.Map<ReviewGrade>(g)));
		}
	}
}


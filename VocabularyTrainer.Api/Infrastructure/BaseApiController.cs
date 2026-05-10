using Common.Wrappers;
using Microsoft.AspNetCore.Mvc;
using IDomainResult = Common.Wrappers.IResult;

namespace VocabularyTrainer.Api.Infrastructure
{
	[ApiController]
	public abstract class BaseApiController : ControllerBase
	{
		// IDomainResult: Common.Wrappers.IResult (not Microsoft.AspNetCore.Http.IResult).
		protected IActionResult ResolveFailure(IDomainResult result)
		{
			return result.ErrorKind switch
			{
				ResultErrorKind.Validation => BadRequest(MakeProblem("Validation error.", result.ErrorMessage, 400)),
				ResultErrorKind.NotFound => NotFound(MakeProblem("Not found.", result.ErrorMessage, 404)),
				ResultErrorKind.Conflict => Conflict(MakeProblem("Conflict.", result.ErrorMessage, 409)),
				_ => Problem(detail: result.ErrorMessage, statusCode: 500),
			};
		}

		private static ProblemDetails MakeProblem(string title, string? detail, int status) =>
			new() { Title = title, Detail = detail, Status = status };
	}
}

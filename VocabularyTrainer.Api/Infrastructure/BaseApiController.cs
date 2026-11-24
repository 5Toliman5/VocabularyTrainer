using System.Globalization;
using Common.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using IDomainResult = Common.Wrappers.IResult;

namespace VocabularyTrainer.Api.Infrastructure
{
	[ApiController]
	public abstract class BaseApiController : ControllerBase
	{
		// The user id always comes from the validated access token, never from the request body or query.
		protected int CurrentUserId
		{
			get
			{
				var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
					?? throw new InvalidOperationException("Authenticated request has no 'sub' claim.");

				return int.Parse(subject, CultureInfo.InvariantCulture);
			}
		}

		// IDomainResult: Common.Wrappers.IResult (not Microsoft.AspNetCore.Http.IResult).
		protected IActionResult ResolveFailure(IDomainResult result)
		{
			return result.ErrorKind switch
			{
				ResultErrorKind.Validation => BadRequest(MakeProblem("Validation error.", result.ErrorMessage, 400)),
				ResultErrorKind.NotFound => NotFound(MakeProblem("Not found.", result.ErrorMessage, 404)),
				ResultErrorKind.Conflict => Conflict(MakeProblem("Conflict.", result.ErrorMessage, 409)),
				ResultErrorKind.Unauthorized => Unauthorized(MakeProblem("Unauthorized.", result.ErrorMessage, 401)),
				ResultErrorKind.TooManyRequests => StatusCode(429, MakeProblem("Too many requests.", result.ErrorMessage, 429)),
				_ => Problem(detail: result.ErrorMessage, statusCode: 500),
			};
		}

		private static ProblemDetails MakeProblem(string title, string? detail, int status) =>
			new() { Title = title, Detail = detail, Status = status };
	}
}

using Common.Web.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Common.Web.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult ResolveFailure(IApiResult result)
        {
            return result.ErrorType switch
            {
                ApiErrorType.Validation    => BadRequest(MakeProblem("Validation error.", result.ErrorMessage, 400)),
                ApiErrorType.NotFound      => NotFound(MakeProblem("Not found.", result.ErrorMessage, 404)),
                ApiErrorType.Conflict      => Conflict(MakeProblem("Conflict.", result.ErrorMessage, 409)),
                ApiErrorType.Forbidden     => StatusCode(403, MakeProblem("Forbidden.", result.ErrorMessage, 403)),
                _                          => Problem(detail: result.ErrorMessage, statusCode: 500),
            };
        }

        private static ProblemDetails MakeProblem(string title, string? detail, int status)
        {
            return new() { Title = title, Detail = detail, Status = status };
        }
    }
}

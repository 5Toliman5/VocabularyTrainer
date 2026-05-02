using AutoMapper;
using Common.Web.Controllers;
using Common.Web.Wrappers;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Contract.Users;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController(IUserRepository repository, IMapper mapper) : BaseApiController
    {
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetByName(string userName)
        {
            var result = await repository.GetUserAsync(userName);
            if (!result.Successful)
                return ResolveFailure(ApiResult.Failure(result.ErrorMessage!, ApiErrorType.NotFound));

            return Ok(mapper.Map<UserResponse>(result.Value));
        }
    }
}

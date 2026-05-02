using AutoMapper;
using Common.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Contract.Users;
using VocabularyTrainer.Api.BusinessLogic.Services;

namespace VocabularyTrainer.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController(IApiUserService service, IMapper mapper) : BaseApiController
    {
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetByName(string userName)
        {
            var result = await service.GetAsync(userName);
            if (!result.Successful)
                return ResolveFailure(result);

            return Ok(mapper.Map<UserResponse>(result.Value));
        }
    }
}

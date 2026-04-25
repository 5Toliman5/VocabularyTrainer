using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Contract.Users;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserRepository repository, IMapper mapper) : ControllerBase
    {
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetByName(string userName)
        {
            var user = await repository.GetUserAsync(userName);
            return user is null ? NotFound() : Ok(mapper.Map<UserResponse>(user));
        }
    }
}

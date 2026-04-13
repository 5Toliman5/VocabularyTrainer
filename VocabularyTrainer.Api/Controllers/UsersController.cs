using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.Contract.Users;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.Api.Controllers
{
    /// <summary>Provides user lookup operations.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserRepository repository, IMapper mapper) : ControllerBase
    {
        /// <summary>Returns the user with the specified username, or 404 if not found.</summary>
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetByName(string userName)
        {
            var user = await repository.GetUserAsync(userName);
            return user is null ? NotFound() : Ok(mapper.Map<UserResponse>(user));
        }
    }
}

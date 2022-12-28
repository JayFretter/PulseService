using BiscuitService.Domain.Handlers;
using BiscuitService.Mappers;
using BiscuitService.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiscuitService.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _handler;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserHandler handler, ILogger<UserController> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserQuery newUser)
        {
            _logger.LogInformation("Creating new user {name}", newUser.Username);

            try
            {
                if (await _handler.UsernameIsTakenAsync(newUser.Username))
                {
                    return BadRequest("Username is taken");
                }

                var domainUser = newUser.ToDomain();
                await _handler.CreateUserAsync(domainUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user {name}", newUser.Username);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }
    }
}

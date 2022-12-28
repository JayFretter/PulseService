using BiscuitService.Mappers;
using BiscuitService.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiscuitService.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateUser([FromBody] CreateUserQuery newUser)
        {
            _logger.LogInformation("Creating new user {name}", newUser.Username);

            try
            {
                var domainUser = newUser.ToDomain();
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

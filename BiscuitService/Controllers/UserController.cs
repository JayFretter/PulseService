using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Handlers;
using BiscuitService.Mappers;
using BiscuitService.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiscuitService.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _handler;
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserHandler handler, ITokenProvider tokenProvider, ILogger<UserController> logger)
        {
            _handler = handler;
            _tokenProvider = tokenProvider;
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogInUser([FromBody] LogInUserQuery logInQuery)
        {
            _logger.LogInformation("Attempting login for user {name}", logInQuery.Username);

            try
            {
                var user = await _handler.GetUserByCredentialsAsync(logInQuery.ToDomain());

                if (user is null)
                {
                    return BadRequest("Could not find a user with the given username/password");
                }

                var authenticationToken = _tokenProvider.GenerateToken(user);

                return Ok(authenticationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed login for user {name}", logInQuery.Username);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}

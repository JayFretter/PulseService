using PulseService.Domain.Adapters;
using PulseService.Domain.Handlers;
using PulseService.Mappers;
using PulseService.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseService.Models.Responses;

namespace PulseService.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _handler;
        private readonly ITokenManager _tokenManager;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserHandler handler, ITokenManager tokenProvider, ILogger<UserController> logger)
        {
            _handler = handler;
            _tokenManager = tokenProvider;
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

                var successResponse = new LoginSuccessfulResponse
                {
                    Token = _tokenManager.GenerateToken(user)
                };

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed login for user {name}", logInQuery.Username);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}

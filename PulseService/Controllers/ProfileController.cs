using Microsoft.AspNetCore.Mvc;
using PulseService.Domain.Exceptions;
using PulseService.Domain.Handlers;

namespace PulseService.Controllers;

[ApiController]
[Route("profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileHandler _handler;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IProfileHandler handler, ILogger<ProfileController> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser([FromRoute] string username, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _handler.GetProfileByUsername(username, cancellationToken);
            return Ok(profile);
        }
        catch (PulseException ex)
        {
            _logger.LogError(ex, "Error occured while getting profile: {error}", ex.Message);
            return Problem($"Could not get profile for user {username}", statusCode: 500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown error occured while getting profile: {error}", ex.Message);
            return Problem($"Could not get profile for user {username}. An unknown error occurred.", statusCode: 500);
        }
    }
}
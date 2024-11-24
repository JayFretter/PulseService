using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseService.Domain.Adapters;
using PulseService.Domain.Handlers;
using PulseService.Domain.Validation;
using PulseService.Helpers;
using PulseService.Mappers;
using PulseService.Models.Queries;
using PulseService.Models.Responses;

namespace PulseService.Controllers;

[ApiController]
[Authorize]
[Route("pulses")]
public class PulseController : ControllerBase
{
    private readonly IPulseHandler _handler;
    private readonly ITokenManager _tokenManager;
    private readonly IPulseValidationService _pulseValidationService;
    private readonly ILogger<PulseController> _logger;

    public PulseController(IPulseHandler handler, ITokenManager tokenManager, IPulseValidationService pulseValidationService, ILogger<PulseController> logger)
    {
        _handler = handler;
        _tokenManager = tokenManager;
        _pulseValidationService = pulseValidationService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePulse([FromBody]CreatePulseQuery newPulse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new Pulse");

        try
        {
            var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
            var pulse = newPulse.ToDomain(currentUser);
                
            var validationErrors = _pulseValidationService.GetValidationErrorsForNewPulse(pulse);
            if (validationErrors.Any())
            {
                return BadRequest(new ValidationErrorResponse
                {
                    ValidationErrors = validationErrors
                });
            }

            await _handler.CreatePulseAsync(pulse, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle creation of Pulse");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeletePulse([FromQuery] string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting Pulse with ID {id}", id);

        try
        {
            var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
            var successful = await _handler.DeletePulseAsync(id, currentUser.Id, cancellationToken);

            if (!successful)
            {
                return BadRequest("No Pulses deleted. Either you did not create it or it doesn't exist.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Pulse with ID {id}", id);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllPulses(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all Pulses");

        try
        {
            var result = await _handler.GetAllPulsesAsync(cancellationToken);

            return Ok(result.FromDomain());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all Pulses");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetPulse([FromQuery] string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting Pulse with ID {id}", id);

        try
        {
            var result = await _handler.GetPulseAsync(id, cancellationToken);

            return Ok(result.FromDomain());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Pulse with ID {id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
        
    [AllowAnonymous]
    [HttpGet("{pulseId}/currentVote")]
    public async Task<IActionResult> GetUsersCurrentVote([FromRoute] string pulseId, [FromQuery] string username, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting current user's vote on Pulse with ID {id}", pulseId);

        try
        {
            var pulseVote = await _handler.GetCurrentVoteForUser(pulseId, username, cancellationToken);

            return Ok(new CurrentPulseVoteResponse
            {
                CurrentVotedOpinion = pulseVote?.OpinionName,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get current user's vote on Pulse with ID {id}", pulseId);
            return Problem($"Could not get current user's vote for Pulse with ID {pulseId}", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
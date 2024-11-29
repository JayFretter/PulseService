using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseService.Api.Models.Queries;
using PulseService.Api.Models.Responses;
using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Handlers;
using PulseService.Domain.Models;
using PulseService.Domain.Validation;
using PulseService.Api.Helpers;
using PulseService.Api.Mappers;

namespace PulseService.Api.Controllers;

[ApiController]
[Authorize]
[Route("discussions")]
public class DiscussionController : ControllerBase
{
    private readonly IDiscussionHandler _handler;
    private readonly ITokenManager _tokenManager;
    private readonly IArgumentValidationService _argumentValidationService;
    private readonly ILogger<PulseController> _logger;

    public DiscussionController(IDiscussionHandler handler, ITokenManager tokenManager, IArgumentValidationService argumentValidationService, ILogger<PulseController> logger)
    {
        _handler = handler;
        _tokenManager = tokenManager;
        _argumentValidationService = argumentValidationService;
        _logger = logger;
    }

    [HttpPost("arguments")]
    public async Task<IActionResult> AddArgument([FromBody]CreateArgumentQuery newArgumentQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new Pulse argument for Pulse {pulseId}", newArgumentQuery.PulseId);

        try
        {
            var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
            var discussionArgument = newArgumentQuery.ToDomain(currentUser);
                
            var validationErrors = _argumentValidationService.GetValidationErrorsForNewArgument(discussionArgument);
            if (validationErrors.Any())
            {
                return BadRequest(new ValidationErrorResponse
                {
                    ValidationErrors = validationErrors
                        
                });
            }

            await _handler.CreateDiscussionArgumentAsync(discussionArgument, cancellationToken);

        } catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle creation of Pulse argument");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetDiscussionForPulse([FromQuery]string pulseId, [FromQuery] int limit, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting discussion for Pulse {pulseId}", pulseId);

        try 
        { 
            var arguments = await _handler.GetDiscussionForPulseAsync(pulseId, limit, cancellationToken);

            return Ok(arguments);
        } 
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Failed to fetch discussion for Pulse {pulseId}", pulseId);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("arguments/{argumentId}/vote")]
    public async Task<IActionResult> VoteOnArgument([FromRoute]string argumentId, [FromQuery]int voteType, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Voting VoteType {voteType} on argument {argumentId}", voteType, argumentId);

        try
        {
            var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
            var argumentVoteUpdate = new ArgumentVoteUpdateRequest
            {
                ArgumentId = argumentId,
                VoteType = (ArgumentVoteStatus)voteType
            };

            await _handler.VoteOnArgumentAsync(currentUser.Id, argumentVoteUpdate, cancellationToken);
            return Ok();
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Failed to vote VoteType {voteType} on argument {argumentId}", voteType, argumentId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
        
    [HttpGet("arguments/{argumentId}/children")]
    [AllowAnonymous]
    public async Task<IActionResult> GetChildArguments([FromRoute]string argumentId, [FromQuery] int limit, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting child Arguments for Argument {argumentId}", argumentId);

        try 
        { 
            var childArguments = await _handler.GetChildArguments(argumentId, limit, cancellationToken);

            return Ok(new GetChildArgumentsResponse
            {
                ChildArguments = childArguments
            });
        } 
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Failed to fetch child Arguments for Argument {argumentId}", argumentId);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
        
    [HttpDelete("arguments/{argumentId}/delete")]
    public async Task<IActionResult> DeleteArgument([FromRoute]string argumentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Setting Argument {argumentId} to deleted.", argumentId);

        try 
        { 
            var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
            await _handler.SetArgumentToDeletedAsync(currentUser.Id, argumentId, cancellationToken);

            return Ok();
        } 
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Failed to fetch child Arguments for Argument {argumentId}", argumentId);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
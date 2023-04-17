using PulseService.Domain.Adapters;
using PulseService.Domain.Handlers;
using PulseService.Domain.Models;
using PulseService.Helpers;
using PulseService.Mappers;
using PulseService.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PulseService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("discussions")]
    public class DiscussionController : ControllerBase
    {
        private readonly IDiscussionHandler _handler;
        private readonly ITokenManager _tokenManager;
        private readonly ILogger<PulseController> _logger;

        public DiscussionController(IDiscussionHandler handler, ITokenManager tokenManager, ILogger<PulseController> logger)
        {
            _handler = handler;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("add-comment")]
        public async Task<IActionResult> CreatePulseComment([FromBody]CreateCommentQuery newCommentQuery)
        {
            _logger.LogInformation("Creating a new Pulse comment for Pulse {pulseId}", newCommentQuery.PulseId);

            try
            {
                var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
                var discussionComment = newCommentQuery.ToDomain(currentUser);

                await _handler.CreateDiscussionCommentAsync(discussionComment);

            } catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle creation of Pulse comment");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }
    }
}

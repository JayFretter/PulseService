﻿using PulseService.Domain.Adapters;
using PulseService.Domain.Handlers;
using PulseService.Domain.Models;
using PulseService.Helpers;
using PulseService.Mappers;
using PulseService.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseService.Domain.Enums;

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
        public async Task<IActionResult> CreatePulseComment([FromBody]CreateCommentQuery newCommentQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a new Pulse comment for Pulse {pulseId}", newCommentQuery.PulseId);

            try
            {
                var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
                var discussionComment = newCommentQuery.ToDomain(currentUser);

                await _handler.CreateDiscussionCommentAsync(discussionComment, cancellationToken);

            } catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle creation of Pulse comment");

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
                var discussion = await _handler.GetDiscussionForPulseAsync(pulseId, limit, cancellationToken);

                return Ok(discussion);
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to fetch discussion for Pulse {pulseId}", pulseId);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("vote-comment/{commentId}")]
        public async Task<IActionResult> VoteOnPulseComment([FromRoute]string commentId, [FromQuery]int voteType, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Voting VoteType {voteType} on comment {commentId}", voteType, commentId);

            try
            {
                var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
                var commentVoteUpdate = new CommentVoteUpdateRequest
                {
                    CommentId = commentId,
                    VoteType = (CommentVoteStatus)voteType
                };

                await _handler.VoteOnCommentAsync(currentUser.Id, commentVoteUpdate, cancellationToken);
                return Ok();

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to vote VoteType {voteType} on comment {commentId}", voteType, commentId);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

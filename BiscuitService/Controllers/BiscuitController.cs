using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Handlers;
using BiscuitService.Helpers;
using BiscuitService.Mappers;
using BiscuitService.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiscuitService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("biscuits")]
    public class BiscuitController : ControllerBase
    {
        private readonly IBiscuitHandler _handler;
        private readonly ITokenManager _tokenManager;
        private readonly ILogger<BiscuitController> _logger;

        public BiscuitController(IBiscuitHandler handler, ITokenManager tokenManager, ILogger<BiscuitController> logger)
        {
            _handler = handler;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateBiscuit([FromBody]CreateBiscuitQuery newBiscuit)
        {
            _logger.LogInformation("Creating a new Biscuit");

            try
            {
                var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
                var biscuit = newBiscuit.ToDomain(currentUser.Id);

                await _handler.CreateBiscuitAsync(biscuit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle creation of Biscuit");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteBiscuit([FromQuery]string id)
        {
            _logger.LogInformation("Deleting Biscuit with ID {id}", id);

            try
            {
                var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
                var successful = await _handler.DeleteBiscuitAsync(id, currentUser.Id);

                if (!successful)
                {
                    return BadRequest("No Biscuits deleted. Either you did not create it or it doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Biscuit with ID {id}", id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllBiscuits()
        {
            _logger.LogInformation("Getting all Biscuits");

            try
            {
                var result = await _handler.GetAllBiscuitsAsync();

                return Ok(result.FromDomain());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Biscuits");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

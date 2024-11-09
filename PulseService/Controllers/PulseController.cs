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
    [Route("pulses")]
    public class PulseController : ControllerBase
    {
        private readonly IPulseHandler _handler;
        private readonly ITokenManager _tokenManager;
        private readonly ILogger<PulseController> _logger;

        public PulseController(IPulseHandler handler, ITokenManager tokenManager, ILogger<PulseController> logger)
        {
            _handler = handler;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatePulse([FromBody]CreatePulseQuery newPulse)
        {
            _logger.LogInformation("Creating a new Pulse");

            try
            {
                var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
                var pulse = newPulse.ToDomain(currentUser);

                await _handler.CreatePulseAsync(pulse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle creation of Pulse");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeletePulse([FromQuery] string id)
        {
            _logger.LogInformation("Deleting Pulse with ID {id}", id);

            try
            {
                var currentUser = _tokenManager.GetUserFromToken(Request.GetBearerToken());
                var successful = await _handler.DeletePulseAsync(id, currentUser.Id);

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
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllPulses()
        {
            _logger.LogInformation("Getting all Pulses");

            try
            {
                var result = await _handler.GetAllPulsesAsync();

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
        public async Task<IActionResult> GetPulse([FromQuery] string id)
        {
            _logger.LogInformation("Getting Pulse with ID {id}", id);

            try
            {
                var result = await _handler.GetPulseAsync(id);

                return Ok(result.FromDomain());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Pulse with ID {id}", id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

using BiscuitService.Domain.Handlers;
using BiscuitService.Mappers;
using BiscuitService.Models;
using Microsoft.AspNetCore.Mvc;

namespace BiscuitService.Controllers
{
    [ApiController]
    [Route("biscuits")]
    public class BiscuitController: ControllerBase
    {
        private readonly IBiscuitHandler _handler;
        private readonly ILogger<BiscuitController> _logger;

        public BiscuitController(IBiscuitHandler handler, ILogger<BiscuitController> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody]CreateBiscuitQuery newBiscuit)
        {
            _logger.LogInformation("Creating a new Biscuit");

            try
            {
                var domainBiscuit = newBiscuit.ToDomain();
                await _handler.CreateBiscuitAsync(domainBiscuit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle creation of Biscuit");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Create([FromQuery]string id)
        {
            _logger.LogInformation("Deleting Biscuit with ID {id}", id);

            try
            {
                await _handler.DeleteBiscuitAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Biscuit with ID {id}", id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }
    }
}

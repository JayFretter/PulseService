using BiscuitService.Domain.Handlers;
using BiscuitService.Domain.Models;
using BiscuitService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
        public async Task<IActionResult> Create([FromBody]BiscuitExternal biscuit)
        {
            _logger.LogInformation("Creating a new Biscuit");

            try
            {
                var domainBiscuit = BiscuitExternal.ToDomain(biscuit);

                await _handler.CreateBiscuitAsync(domainBiscuit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle creation of Biscuit");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }
    }
}

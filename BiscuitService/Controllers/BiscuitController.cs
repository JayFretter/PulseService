using BiscuitService.Domain.Handlers;
using BiscuitService.Domain.Models;
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
        public async Task<bool> Create()
        {
            _logger.LogInformation("Creating a new Biscuit");

            return await _handler.CreateBiscuitAsync(new Biscuit());
        }
    }
}

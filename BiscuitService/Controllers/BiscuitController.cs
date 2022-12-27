using Microsoft.AspNetCore.Mvc;

namespace BiscuitService.Controllers
{
    [ApiController]
    [Route("biscuits")]
    public class BiscuitController: ControllerBase
    {
        private readonly ILogger<BiscuitController> _logger;

        public BiscuitController(ILogger<BiscuitController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public bool Create()
        {
            _logger.LogInformation("Creating a new Biscuit");

            return true;
        }
    }
}

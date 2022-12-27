using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public class BiscuitHandler : IBiscuitHandler
    {
        public async Task<bool> CreateBiscuitAsync(Biscuit biscuit)
        {
            return true;
        }
    }
}

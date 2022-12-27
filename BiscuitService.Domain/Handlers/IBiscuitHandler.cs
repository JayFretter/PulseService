using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public interface IBiscuitHandler
    {
        public Task<bool> CreateBiscuitAsync(Biscuit biscuit);
    }
}

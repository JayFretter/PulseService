using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public interface IBiscuitHandler
    {
        public Task CreateBiscuitAsync(Biscuit biscuit);
        public Task DeleteBiscuitAsync(string id);
    }
}

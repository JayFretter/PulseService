using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Adapters
{
    public interface IBiscuitRepository
    {
        public Task AddBiscuitAsync(Biscuit biscuit);
        public Task DeleteBiscuitAsync(string id);
    }
}

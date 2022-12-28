using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Adapters
{
    public interface IBiscuitRepository
    {
        Task AddBiscuitAsync(Biscuit biscuit);
        Task DeleteBiscuitAsync(string id);
        Task<IEnumerable<Biscuit>> GetAllBiscuitsAsync();
    }
}

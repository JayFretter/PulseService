using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public interface IBiscuitHandler
    {
        Task CreateBiscuitAsync(Biscuit biscuit);
        Task DeleteBiscuitAsync(string id);
        Task<IEnumerable<Biscuit>> GetAllBiscuitsAsync();
    }
}

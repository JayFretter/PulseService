using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public interface IBiscuitHandler
    {
        Task CreateBiscuitAsync(Biscuit biscuit);
        Task<bool> DeleteBiscuitAsync(string id, string currentUserId);
        Task<IEnumerable<Biscuit>> GetAllBiscuitsAsync();
    }
}

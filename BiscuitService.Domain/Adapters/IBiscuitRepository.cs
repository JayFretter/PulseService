using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Adapters
{
    public interface IBiscuitRepository
    {
        Task AddBiscuitAsync(Biscuit biscuit);
        Task<bool> DeleteBiscuitAsync(string id, string currentUserId);
        Task<IEnumerable<Biscuit>> GetAllBiscuitsAsync();
        Task UpdateBiscuitVoteAsync(VoteUpdate voteUpdate);
    }
}

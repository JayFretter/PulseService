using PulseService.Domain.Models;

namespace PulseService.Domain.Adapters
{
    public interface IPulseRepository
    {
        Task AddPulseAsync(Pulse pulse);
        Task<bool> DeletePulseAsync(string id, string currentUserId);
        Task<IEnumerable<Pulse>> GetAllPulsesAsync();
        Task UpdatePulseVoteAsync(VoteUpdate voteUpdate);
    }
}

using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public interface IPulseHandler
    {
        Task CreatePulseAsync(Pulse pulse);
        Task<bool> DeletePulseAsync(string id, string currentUserId);
        Task<IEnumerable<Pulse>> GetAllPulsesAsync();
        Task UpdatePulseVoteAsync(VoteUpdate voteUpdate);
    }
}

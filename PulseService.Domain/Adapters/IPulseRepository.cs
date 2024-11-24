using PulseService.Domain.Models;

namespace PulseService.Domain.Adapters
{
    public interface IPulseRepository
    {
        Task AddPulseAsync(Pulse pulse, CancellationToken cancellationToken);
        Task<bool> DeletePulseAsync(string id, string currentUserId, CancellationToken cancellationToken);
        Task<IEnumerable<Pulse>> GetAllPulsesAsync(CancellationToken cancellationToken);
        Task<Pulse?> GetPulseAsync(string id, CancellationToken cancellationToken);
        Task UpdatePulseVoteAsync(VoteUpdate voteUpdate, CancellationToken cancellationToken);
        Task<IEnumerable<Pulse>> GetPulsesByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}

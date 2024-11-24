using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers;

public interface IPulseHandler
{
    Task CreatePulseAsync(Pulse pulse, CancellationToken cancellationToken);
    Task<bool> DeletePulseAsync(string id, string currentUserId, CancellationToken cancellationToken);
    Task<IEnumerable<Pulse>> GetAllPulsesAsync(CancellationToken cancellationToken);
    Task<Pulse> GetPulseAsync(string id, CancellationToken cancellationToken);
    Task UpdatePulseVoteAsync(VoteUpdate voteUpdate, CancellationToken cancellationToken);
    Task<PulseVote?> GetCurrentVoteForUser(string pulseId, string username, CancellationToken cancellationToken);
}
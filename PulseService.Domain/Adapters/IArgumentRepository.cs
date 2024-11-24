using PulseService.Domain.Models;

namespace PulseService.Domain.Adapters;

public interface IArgumentRepository
{
    Task AddArgumentAsync(DiscussionArgument discussionArgument, CancellationToken cancellationToken);
    Task<IEnumerable<DiscussionArgument>> GetArgumentsForPulseIdAsync(string pulseId, int limit, CancellationToken cancellationToken);
    Task<IEnumerable<DiscussionArgument>> GetChildrenOfArgumentIdAsync(string pulseId, int limit, CancellationToken cancellationToken);
    Task AdjustArgumentVotesAsync(string argumentId, int upvoteIncrement, int downvoteIncrement, CancellationToken cancellationToken);
    Task SetArgumentToDeletedAsync(string userId, string argumentId, CancellationToken cancellationToken);
}
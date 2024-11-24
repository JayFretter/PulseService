using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public interface IDiscussionHandler
    {
        Task CreateDiscussionArgumentAsync(DiscussionArgument discussionArgument, CancellationToken cancellationToken);
        Task<IEnumerable<CollatedDiscussionArgument>> GetDiscussionForPulseAsync(string pulseId, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<CollatedDiscussionArgument>> GetChildArguments(string argumentId, int limit, CancellationToken cancellationToken);
        Task<Discussion> GetDiscussionForPulseLegacyAsync(string pulseId, int limit, CancellationToken cancellationToken);
        Task VoteOnArgumentAsync(string userId, ArgumentVoteUpdateRequest argumentVoteUpdate, CancellationToken cancellationToken);
        Task SetArgumentToDeletedAsync(string userId, string argumentId, CancellationToken cancellationToken);
    }
}

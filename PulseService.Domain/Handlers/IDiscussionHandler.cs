using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public interface IDiscussionHandler
    {
        Task CreateDiscussionCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken);
        Task<IEnumerable<CollatedDiscussionComment>> GetDiscussionForPulseAsync(string pulseId, int limit, CancellationToken cancellationToken);
        Task<Discussion> GetDiscussionForPulseLegacyAsync(string pulseId, int limit, CancellationToken cancellationToken);
        Task VoteOnCommentAsync(string userId, CommentVoteUpdateRequest commentVoteUpdate, CancellationToken cancellationToken);
    }
}

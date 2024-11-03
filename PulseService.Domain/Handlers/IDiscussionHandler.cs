using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public interface IDiscussionHandler
    {
        Task CreateDiscussionCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken);
        Task<Discussion?> GetDiscussionForPulseAsync(string pulseId, int limit, CancellationToken cancellationToken);
        Task VoteOnCommentAsync(CommentVoteUpdate commentVoteUpdate, CancellationToken cancellationToken);
    }
}

using PulseService.Domain.Enums;
using PulseService.Domain.Models;

namespace PulseService.Domain.Adapters
{
    public interface ICommentRepository
    {
        Task AddCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken);
        Task<IEnumerable<DiscussionComment>> GetCommentsForPulseIdAsync(string pulseId, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<DiscussionComment>> GetChildrenOfCommentIdAsync(string pulseId, CancellationToken cancellationToken);
        Task IncrementCommentUpvotesAsync(string commentId, int increment, CancellationToken cancellationToken);
    }
}

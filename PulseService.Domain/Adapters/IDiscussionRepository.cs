using PulseService.Domain.Models;

namespace PulseService.Domain.Adapters
{
    public interface IDiscussionRepository
    {
        Task AddDiscussionCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken);
    }
}

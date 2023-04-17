using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public interface IDiscussionHandler
    {
        Task CreateDiscussionCommentAsync(DiscussionComment discussionComment);
    }
}

using PulseService.Domain.Models;

namespace PulseService.Domain.Mappers
{
    internal static class DiscussionCommentMapper
    {
        public static CollatedDiscussionComment ToCollatedComment(this DiscussionComment comment)
        {
            return new CollatedDiscussionComment
            {
                Id = comment.Id,
                UserId = comment.UserId,
                Username = comment.Username,
                OpinionName = comment.OpinionName,
                CommentBody = comment.CommentBody,
                PulseId = comment.PulseId,
                Upvotes = comment.Upvotes,
                Downvotes = comment.Downvotes,
                Children = Array.Empty<CollatedDiscussionComment>()
            };
        }
    }
}

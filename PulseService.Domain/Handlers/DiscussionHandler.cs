using PulseService.Domain.Adapters;
using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public class DiscussionHandler : IDiscussionHandler
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPulseRepository _pulseRepository;
        public DiscussionHandler(ICommentRepository commentRepository, IPulseRepository pulseRepository)
        {
            _commentRepository = commentRepository;
            _pulseRepository = pulseRepository;
        }

        public async Task CreateDiscussionCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken)
        {
            if (await _pulseRepository.GetPulseAsync(discussionComment.PulseId) is not null)
            {
                await _commentRepository.AddCommentAsync(discussionComment, cancellationToken);
            }
        }

        public async Task<Discussion?> GetDiscussionForPulseAsync(string pulseId, int limit, CancellationToken cancellationToken)
        {
            var topLevelComments = await _commentRepository.GetCommentsForPulseIdAsync(pulseId, limit, cancellationToken);

            var collatedComments = topLevelComments.Select(c => new CollatedDiscussionComment
            {
                Id = c.Id,
                UserId = c.UserId,
                Username = c.Username,
                OpinionName = c.OpinionName,
                CommentBody = c.CommentBody,
                PulseId = c.PulseId,
                Upvotes = c.Upvotes,
                Downvotes = c.Downvotes,
                Children = Array.Empty<CollatedDiscussionComment>()
            });

            var groupedComments = collatedComments.GroupBy(c => c.OpinionName);

            var opinionThreads = groupedComments.Select(cg => new OpinionThread
            {
                OpinionName = cg.Key,
                DiscussionComments = cg
            });

            return new Discussion
            {
                OpinionThreads = opinionThreads
            };
        }

        public Task VoteOnCommentAsync(CommentVoteUpdate commentVoteUpdate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

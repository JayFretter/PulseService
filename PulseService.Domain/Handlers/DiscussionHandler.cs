using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public class DiscussionHandler : IDiscussionHandler
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPulseRepository _pulseRepository;
        private readonly IUserRepository _userRepository;
        public DiscussionHandler(ICommentRepository commentRepository, IPulseRepository pulseRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _pulseRepository = pulseRepository;
            _userRepository = userRepository;
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

        public async Task VoteOnCommentAsync(string userId, CommentVoteUpdate commentVoteUpdate, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (currentUser == null || currentUser.Id == null)
            {
                throw new Exception("User cannot be null when voting on comment"); // TODO: custom ex
            }

            var currentVoteStatusOnComment = currentUser.CommentVotes
                .FirstOrDefault(cv => cv.CommentId == commentVoteUpdate.CommentId)?
                .VoteStatus;

            if (currentVoteStatusOnComment != null)
            {
                await AdjustVotesForCommentWithExistingUserVote(
                    userId, commentVoteUpdate.CommentId, currentVoteStatusOnComment.Value, commentVoteUpdate.VoteType, cancellationToken);
                
                return;
            }

            if (commentVoteUpdate.VoteType == CommentVoteStatus.Upvote)
            {
                await UpdateVotes(commentVoteUpdate.CommentId, userId, 1, 0, CommentVoteStatus.Upvote, cancellationToken);
            } 
            else if (commentVoteUpdate.VoteType == CommentVoteStatus.Downvote)
            {
                await UpdateVotes(commentVoteUpdate.CommentId, userId, 0, -1, CommentVoteStatus.Downvote, cancellationToken);
            }
        }

        private async Task AdjustVotesForCommentWithExistingUserVote(string userId, string commentId, CommentVoteStatus existingVote, CommentVoteStatus voteUpdate, CancellationToken cancellationToken)
        {
            if (voteUpdate == CommentVoteStatus.Upvote && existingVote == CommentVoteStatus.Downvote)
            {
                await UpdateVotes(commentId, userId, upvoteChange: 1, downvoteChange: -1, CommentVoteStatus.Upvote, cancellationToken);
            } 
            else if (voteUpdate == CommentVoteStatus.Downvote && existingVote == CommentVoteStatus.Upvote)
            {
                await UpdateVotes(commentId, userId, upvoteChange: -1, downvoteChange: 1, CommentVoteStatus.Downvote, cancellationToken);
            }
            else if (voteUpdate == CommentVoteStatus.Neutral)
            {
                int upvoteChange = existingVote == CommentVoteStatus.Upvote ? -1 : 0;
                int downvoteChange = existingVote == CommentVoteStatus.Downvote ? -1 : 0;

                await RemoveVote(commentId, userId, upvoteChange, downvoteChange, cancellationToken);
            }
        }

        private async Task UpdateVotes(string commentId, string userId, int upvoteChange, int downvoteChange, CommentVoteStatus newStatus, CancellationToken cancellationToken)
        {
            await _commentRepository.AdjustCommentVotesAsync(commentId, upvoteIncrement: upvoteChange, downvoteIncrement: downvoteChange, cancellationToken);
            await _userRepository.UpdateCommentVoteStatusAsync(userId, commentId, newStatus, cancellationToken);
        }

        private async Task RemoveVote(string commentId, string userId, int upvoteChange, int downvoteChange, CancellationToken cancellationToken)
        {
            await _commentRepository.AdjustCommentVotesAsync(commentId, upvoteIncrement: upvoteChange, downvoteIncrement: downvoteChange, cancellationToken);
            await _userRepository.RemoveCommentVoteStatusAsync(userId, commentId, cancellationToken);
        }
    }
}

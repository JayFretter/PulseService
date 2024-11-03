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
                throw new InvalidDataException($"No user found for userId {userId} when voting on comment.");
            }

            var currentVoteForComment = currentUser.CommentVotes.FirstOrDefault(cv => cv.CommentId == commentVoteUpdate.CommentId)?.VoteStatus;
            if (currentVoteForComment != null)
            {
                await UpdateVotesForCommentWithExistingVoteAsync(userId, commentVoteUpdate.CommentId, currentVoteForComment.Value,
                    commentVoteUpdate.VoteType, cancellationToken);
                return;
            }

            if (commentVoteUpdate.VoteType == CommentVoteStatus.Upvote)
            {
                await UpdateVotesAsync(commentVoteUpdate.CommentId, userId, 1, 0, CommentVoteStatus.Upvote, cancellationToken);
            } 
            else if (commentVoteUpdate.VoteType == CommentVoteStatus.Downvote)
            {
                await UpdateVotesAsync(commentVoteUpdate.CommentId, userId, 0, -1, CommentVoteStatus.Downvote, cancellationToken);
            }
        }

        private async Task UpdateVotesForCommentWithExistingVoteAsync(string userId, string commentId, CommentVoteStatus existingVote, CommentVoteStatus newVote, CancellationToken cancellationToken)
        {
            if (newVote == CommentVoteStatus.Upvote && existingVote == CommentVoteStatus.Downvote)
            {
                await UpdateVotesAsync(commentId, userId, upvoteChange: 1, downvoteChange: -1, CommentVoteStatus.Upvote, cancellationToken);
            } 
            else if (newVote == CommentVoteStatus.Downvote && existingVote == CommentVoteStatus.Upvote)
            {
                await UpdateVotesAsync(commentId, userId, upvoteChange: -1, downvoteChange: 1, CommentVoteStatus.Downvote, cancellationToken);
            }
            else if (newVote == CommentVoteStatus.Neutral && existingVote == CommentVoteStatus.Upvote)
            {
                await RemoveVoteAsync(commentId, userId, -1, 0, cancellationToken);
            }
            else if (newVote == CommentVoteStatus.Neutral && existingVote == CommentVoteStatus.Downvote)
            {
                await RemoveVoteAsync(commentId, userId, 0, -1, cancellationToken);
            }
        }

        private Task UpdateVotesAsync(string commentId, string userId, int upvoteChange, int downvoteChange, CommentVoteStatus newStatus, CancellationToken cancellationToken)
        {
            var updateTasks = new Task[]
            {
                _commentRepository.AdjustCommentVotesAsync(commentId, upvoteIncrement: upvoteChange, downvoteIncrement: downvoteChange, cancellationToken),
                _userRepository.UpdateCommentVoteStatusAsync(userId, commentId, newStatus, cancellationToken)
            };

            return Task.WhenAll(updateTasks);
        }

        private Task RemoveVoteAsync(string commentId, string userId, int upvoteChange, int downvoteChange, CancellationToken cancellationToken)
        {
            var updateTasks = new Task[]
            {
                _commentRepository.AdjustCommentVotesAsync(commentId, upvoteIncrement: upvoteChange, downvoteIncrement: downvoteChange, cancellationToken),
                _userRepository.RemoveCommentVoteStatusAsync(userId, commentId, cancellationToken)
            };

            return Task.WhenAll(updateTasks);
        }
    }
}

using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Exceptions;
using PulseService.Domain.Mappers;
using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public class DiscussionHandler : IDiscussionHandler
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPulseRepository _pulseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPulseHandler _pulseHandler;

        public DiscussionHandler(ICommentRepository commentRepository, IPulseRepository pulseRepository, IUserRepository userRepository, IPulseHandler pulseHandler)
        {
            _commentRepository = commentRepository;
            _pulseRepository = pulseRepository;
            _userRepository = userRepository;
            _pulseHandler = pulseHandler;
        }

        public async Task CreateDiscussionCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken)
        {
            if (await _pulseRepository.GetPulseAsync(discussionComment.PulseId) is not null)
            {
                await _commentRepository.AddCommentAsync(discussionComment, cancellationToken);

                var pulseVoteUpdate = new VoteUpdate
                {
                    CurrentUserId = discussionComment.UserId,
                    PulseId = discussionComment.PulseId,
                    VotedOpinion = discussionComment.OpinionName
                };

                await _pulseHandler.UpdatePulseVoteAsync(pulseVoteUpdate, cancellationToken);
            }
        }

        public async Task<IEnumerable<CollatedDiscussionComment>> GetDiscussionForPulseAsync(string pulseId, int limit, CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.GetCommentsForPulseIdAsync(pulseId, limit, cancellationToken);

            var collatedComments = comments
                .Select(c => c.ToCollatedComment());

            return collatedComments;
        }

        public async Task<Discussion> GetDiscussionForPulseLegacyAsync(string pulseId, int limit, CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.GetCommentsForPulseIdAsync(pulseId, limit, cancellationToken);

            var opinionThreads = comments
                .GroupBy(c => c.OpinionName)
                .Select(cg => new OpinionThread
                {
                    OpinionName = cg.Key,
                    DiscussionComments = cg.Select(c => c.ToCollatedComment())
                });

            return new Discussion
            {
                OpinionThreads = opinionThreads
            };
        }

        public async Task VoteOnCommentAsync(string userId, CommentVoteUpdateRequest voteUpdateRequest, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (currentUser?.Id == null)
            {
                throw new MissingDataException($"No user found for userId {userId} when voting on comment.");
            }

            var currentVote = currentUser.CommentVotes.FirstOrDefault(cv => cv.CommentId == voteUpdateRequest.CommentId)?.VoteStatus;

            await ApplyVoteChangeAsync(currentVote, voteUpdateRequest, userId, cancellationToken);
        }

        private async Task ApplyVoteChangeAsync(CommentVoteStatus? currentVote, CommentVoteUpdateRequest voteUpdateRequest, string userId, CancellationToken cancellationToken)
        {
            if (voteUpdateRequest.VoteType == currentVote)
                return;

            (int upvoteChange, int downvoteChange) = CalculateVoteChange(currentVote, voteUpdateRequest.VoteType);

            if (voteUpdateRequest.VoteType == CommentVoteStatus.Neutral)
            {
                await RemoveVoteAsync(voteUpdateRequest.CommentId, userId, upvoteChange, downvoteChange, cancellationToken);
            } else
            {
                await UpdateVoteStatusAsync(voteUpdateRequest.CommentId, userId, upvoteChange, downvoteChange, voteUpdateRequest.VoteType, cancellationToken);
            }
        }

        private static (int upvoteChange, int downvoteChange) CalculateVoteChange(CommentVoteStatus? currentVote, CommentVoteStatus newVote)
        {
            return (currentVote, newVote) switch
            {
                (null, CommentVoteStatus.Upvote) => (1, 0),
                (null, CommentVoteStatus.Downvote) => (0, 1),
                (CommentVoteStatus.Upvote, CommentVoteStatus.Downvote) => (-1, 1),
                (CommentVoteStatus.Downvote, CommentVoteStatus.Upvote) => (1, -1),
                (CommentVoteStatus.Upvote, CommentVoteStatus.Neutral) => (-1, 0),
                (CommentVoteStatus.Downvote, CommentVoteStatus.Neutral) => (0, -1),
                _ => (0, 0)
            };
        }

        private Task UpdateVoteStatusAsync(string commentId, string userId, int upvoteChange, int downvoteChange, CommentVoteStatus newStatus, CancellationToken cancellationToken)
        {
            var updateTasks = new Task[]
            {
                _commentRepository.AdjustCommentVotesAsync(commentId, upvoteChange, downvoteChange, cancellationToken),
                _userRepository.UpdateCommentVoteStatusAsync(userId, commentId, newStatus, cancellationToken)
            };

            return Task.WhenAll(updateTasks);
        }

        private Task RemoveVoteAsync(string commentId, string userId, int upvoteChange, int downvoteChange, CancellationToken cancellationToken)
        {
            var updateTasks = new Task[]
            {
                _commentRepository.AdjustCommentVotesAsync(commentId, upvoteChange, downvoteChange, cancellationToken),
                _userRepository.RemoveCommentVoteStatusAsync(userId, commentId, cancellationToken)
            };

            return Task.WhenAll(updateTasks);
        }
    }

}

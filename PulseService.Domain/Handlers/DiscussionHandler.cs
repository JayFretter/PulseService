using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

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
                switch (commentVoteUpdate.VoteType)
                {
                    case CommentVoteType.Upvote:
                        if (currentVoteStatusOnComment == CommentVoteStatus.Downvoted)
                        {
                            await _commentRepository.AdjustCommentVotesAsync(commentVoteUpdate.CommentId,
                                upvoteIncrement: 1, downvoteIncrement: -1, cancellationToken);
                            await _userRepository.UpdateCommentVoteStatusAsync(currentUser.Id, commentVoteUpdate.CommentId, CommentVoteStatus.Upvoted, cancellationToken);
                        }
                        return;
                    case CommentVoteType.Downvote:
                        if (currentVoteStatusOnComment == CommentVoteStatus.Upvoted)
                        {
                            await _commentRepository.AdjustCommentVotesAsync(commentVoteUpdate.CommentId,
                                upvoteIncrement: -1, downvoteIncrement: 1, cancellationToken);
                            await _userRepository.UpdateCommentVoteStatusAsync(currentUser.Id, commentVoteUpdate.CommentId, CommentVoteStatus.Downvoted, cancellationToken);
                        }
                        return;
                    case CommentVoteType.Neutral:
                        if (currentVoteStatusOnComment == CommentVoteStatus.Upvoted)
                        {
                            await _commentRepository.AdjustCommentVotesAsync(commentVoteUpdate.CommentId,
                                upvoteIncrement: -1, downvoteIncrement: 0, cancellationToken);
                            await _userRepository.RemoveCommentVoteStatusAsync(currentUser.Id, commentVoteUpdate.CommentId, cancellationToken);
                        }
                        else
                        {
                            await _commentRepository.AdjustCommentVotesAsync(commentVoteUpdate.CommentId,
                                upvoteIncrement: 0, downvoteIncrement: -1, cancellationToken);
                            await _userRepository.RemoveCommentVoteStatusAsync(currentUser.Id, commentVoteUpdate.CommentId, cancellationToken);
                        }
                        return;
                    default:
                        return;
                }
            }

            if (commentVoteUpdate.VoteType == CommentVoteType.Upvote)
            {
                await _commentRepository.AdjustCommentVotesAsync(commentVoteUpdate.CommentId,
                                upvoteIncrement: 1, downvoteIncrement: 0, cancellationToken);
                await _userRepository.UpdateCommentVoteStatusAsync(currentUser.Id, commentVoteUpdate.CommentId, CommentVoteStatus.Upvoted, cancellationToken);
            } 
            else if (commentVoteUpdate.VoteType == CommentVoteType.Downvote)
            {
                await _commentRepository.AdjustCommentVotesAsync(commentVoteUpdate.CommentId,
                                upvoteIncrement: 0, downvoteIncrement: 1, cancellationToken);
                await _userRepository.UpdateCommentVoteStatusAsync(currentUser.Id, commentVoteUpdate.CommentId, CommentVoteStatus.Downvoted, cancellationToken);
            }
        }
    }
}

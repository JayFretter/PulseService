using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Exceptions;
using PulseService.Domain.Mappers;
using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public class DiscussionHandler : IDiscussionHandler
    {
        private readonly IArgumentRepository _argumentRepository;
        private readonly IPulseRepository _pulseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPulseHandler _pulseHandler;

        public DiscussionHandler(IArgumentRepository argumentRepository, IPulseRepository pulseRepository, IUserRepository userRepository, IPulseHandler pulseHandler)
        {
            _argumentRepository = argumentRepository;
            _pulseRepository = pulseRepository;
            _userRepository = userRepository;
            _pulseHandler = pulseHandler;
        }

        public async Task CreateDiscussionArgumentAsync(DiscussionArgument discussionArgument, CancellationToken cancellationToken)
        {
            if (await _pulseRepository.GetPulseAsync(discussionArgument.PulseId, cancellationToken) != null)
            {
                if (discussionArgument.ParentArgumentId != null)
                {
                    await CreateDiscussionArgumentResponseAsync(discussionArgument, cancellationToken);
                    return;
                }
                
                await _argumentRepository.AddArgumentAsync(discussionArgument, cancellationToken);

                var pulseVoteUpdate = new VoteUpdate
                {
                    CurrentUserId = discussionArgument.UserId,
                    PulseId = discussionArgument.PulseId,
                    VotedOpinion = discussionArgument.OpinionName
                };

                await _pulseHandler.UpdatePulseVoteAsync(pulseVoteUpdate, cancellationToken);
            }
        }

        public async Task<IEnumerable<CollatedDiscussionArgument>> GetDiscussionForPulseAsync(string pulseId, int limit, CancellationToken cancellationToken)
        {
            var arguments = await _argumentRepository.GetArgumentsForPulseIdAsync(pulseId, limit, cancellationToken);

            var collatedArguments = arguments
                .Select(c => c.ToCollatedArgument());

            return collatedArguments;
        }

        public async Task<IEnumerable<CollatedDiscussionArgument>> GetChildArguments(string argumentId, int limit, CancellationToken cancellationToken)
        {
            var childArguments = await _argumentRepository.GetChildrenOfArgumentIdAsync(argumentId, limit, cancellationToken);

            var collatedArguments = childArguments
                .Select(c => c.ToCollatedArgument());

            return collatedArguments;
        }

        public async Task<Discussion> GetDiscussionForPulseLegacyAsync(string pulseId, int limit, CancellationToken cancellationToken)
        {
            var arguments = await _argumentRepository.GetArgumentsForPulseIdAsync(pulseId, limit, cancellationToken);

            var opinionThreads = arguments
                .GroupBy(c => c.OpinionName)
                .Select(cg => new OpinionThread
                {
                    OpinionName = cg.Key,
                    DiscussionArguments = cg.Select(c => c.ToCollatedArgument())
                });

            return new Discussion
            {
                OpinionThreads = opinionThreads
            };
        }

        public async Task VoteOnArgumentAsync(string userId, ArgumentVoteUpdateRequest voteUpdateRequest, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (currentUser?.Id == null)
            {
                throw new MissingDataException($"No user found for userId {userId} when voting on argument.");
            }

            var currentVote = currentUser.ArgumentVotes.FirstOrDefault(cv => cv.ArgumentId == voteUpdateRequest.ArgumentId)?.VoteStatus;

            await ApplyVoteChangeAsync(currentVote, voteUpdateRequest, userId, cancellationToken);
        }

        public async Task SetArgumentToDeletedAsync(string userId, string argumentId, CancellationToken cancellationToken)
        {
            await _argumentRepository.SetArgumentToDeletedAsync(userId, argumentId, cancellationToken);
        }
        
        private async Task CreateDiscussionArgumentResponseAsync(DiscussionArgument discussionArgument, CancellationToken cancellationToken)
        {
            var currentVote =
                await _userRepository.GetCurrentPulseVote(discussionArgument.UserId, discussionArgument.PulseId, cancellationToken);
            
            if (currentVote == null) // They haven't already voted on the Pulse, disallow replying
                return;
            
            discussionArgument.OpinionName = currentVote.OpinionName;
            await _argumentRepository.AddArgumentAsync(discussionArgument, cancellationToken);
        }

        private async Task ApplyVoteChangeAsync(ArgumentVoteStatus? currentVote, ArgumentVoteUpdateRequest voteUpdateRequest, string userId, CancellationToken cancellationToken)
        {
            if (voteUpdateRequest.VoteType == currentVote)
                return;

            (int upvoteChange, int downvoteChange) = CalculateVoteChange(currentVote, voteUpdateRequest.VoteType);

            if (voteUpdateRequest.VoteType == ArgumentVoteStatus.Neutral)
            {
                await RemoveVoteAsync(voteUpdateRequest.ArgumentId, userId, upvoteChange, downvoteChange, cancellationToken);
            } else
            {
                await UpdateVoteStatusAsync(voteUpdateRequest.ArgumentId, userId, upvoteChange, downvoteChange, voteUpdateRequest.VoteType, cancellationToken);
            }
        }

        private static (int upvoteChange, int downvoteChange) CalculateVoteChange(ArgumentVoteStatus? currentVote, ArgumentVoteStatus newVote)
        {
            return (currentVote, newVote) switch
            {
                (null, ArgumentVoteStatus.Upvote) => (1, 0),
                (null, ArgumentVoteStatus.Downvote) => (0, 1),
                (ArgumentVoteStatus.Upvote, ArgumentVoteStatus.Downvote) => (-1, 1),
                (ArgumentVoteStatus.Downvote, ArgumentVoteStatus.Upvote) => (1, -1),
                (ArgumentVoteStatus.Upvote, ArgumentVoteStatus.Neutral) => (-1, 0),
                (ArgumentVoteStatus.Downvote, ArgumentVoteStatus.Neutral) => (0, -1),
                _ => (0, 0)
            };
        }

        private Task UpdateVoteStatusAsync(string argumentId, string userId, int upvoteChange, int downvoteChange, ArgumentVoteStatus newStatus, CancellationToken cancellationToken)
        {
            var updateTasks = new Task[]
            {
                _argumentRepository.AdjustArgumentVotesAsync(argumentId, upvoteChange, downvoteChange, cancellationToken),
                _userRepository.UpdateArgumentVoteStatusAsync(userId, argumentId, newStatus, cancellationToken)
            };

            return Task.WhenAll(updateTasks);
        }

        private Task RemoveVoteAsync(string argumentId, string userId, int upvoteChange, int downvoteChange, CancellationToken cancellationToken)
        {
            var updateTasks = new Task[]
            {
                _argumentRepository.AdjustArgumentVotesAsync(argumentId, upvoteChange, downvoteChange, cancellationToken),
                _userRepository.RemoveArgumentVoteStatusAsync(userId, argumentId, cancellationToken)
            };

            return Task.WhenAll(updateTasks);
        }
    }

}

using Moq;
using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Exceptions;
using PulseService.Domain.Handlers;
using PulseService.Domain.Models;

namespace PulseService.Domain.Tests.Unit.Handlers
{
    [TestFixture]
    public class DiscussionHandlerTests
    {
        private Mock<IArgumentRepository> _mockArgumentRepository;
        private Mock<IPulseRepository> _mockPulseRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IPulseHandler> _mockPulseHandler;
        private DiscussionHandler _discussionHandler;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _mockArgumentRepository = new Mock<IArgumentRepository>();
            _mockPulseRepository = new Mock<IPulseRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPulseHandler = new Mock<IPulseHandler>();
            _discussionHandler = new DiscussionHandler(_mockArgumentRepository.Object, _mockPulseRepository.Object, _mockUserRepository.Object, _mockPulseHandler.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Test]
        public async Task CreateDiscussionArgumentAsync_PulseExists_AddsArgumentAndUpdatesPulseVotes()
        {
            // Arrange
            var discussionArgument = new DiscussionArgument { PulseId = "pulse1", OpinionName = "agree" };
            _mockPulseRepository.Setup(pr => pr.GetPulseAsync(discussionArgument.PulseId)).ReturnsAsync(new Pulse());

            // Act
            await _discussionHandler.CreateDiscussionArgumentAsync(discussionArgument, _cancellationToken);

            // Assert
            _mockArgumentRepository.Verify(cr => cr.AddArgumentAsync(discussionArgument, _cancellationToken), Times.Once);
            _mockPulseHandler.Verify(ph => ph.UpdatePulseVoteAsync(It.Is<VoteUpdate>(v =>
                v.PulseId == discussionArgument.PulseId &&
                v.CurrentUserId == discussionArgument.UserId &&
                v.VotedOpinion == discussionArgument.OpinionName
            ), _cancellationToken), Times.Once);
        }

        [Test]
        public async Task CreateDiscussionArgumentAsync_PulseDoesNotExist_DoesNotAddArgument()
        {
            // Arrange
            var discussionArgument = new DiscussionArgument { PulseId = "pulse1" };
            _mockPulseRepository.Setup(pr => pr.GetPulseAsync(discussionArgument.PulseId)).ReturnsAsync((Pulse?)null);

            // Act
            await _discussionHandler.CreateDiscussionArgumentAsync(discussionArgument, _cancellationToken);

            // Assert
            _mockArgumentRepository.Verify(cr => cr.AddArgumentAsync(It.IsAny<DiscussionArgument>(), _cancellationToken), Times.Never);
            _mockPulseHandler.Verify(ph => ph.UpdatePulseVoteAsync(It.IsAny<VoteUpdate>(), _cancellationToken), Times.Never);
        }

        [Test]
        public async Task GetDiscussionForPulseAsync_ReturnsGroupedArgumentsByOpinion()
        {
            // Arrange
            var pulseId = "pulse1";
            var arguments = new List<DiscussionArgument>
            {
                new DiscussionArgument { OpinionName = "Agree", ArgumentBody = "Argument 1" },
                new DiscussionArgument { OpinionName = "Agree", ArgumentBody = "Argument 2" },
                new DiscussionArgument { OpinionName = "Disagree", ArgumentBody = "Argument 3" }
            };
            _mockArgumentRepository.Setup(cr => cr.GetArgumentsForPulseIdAsync(pulseId, It.IsAny<int>(), _cancellationToken)).ReturnsAsync(arguments);

            // Act
            var result = await _discussionHandler.GetDiscussionForPulseLegacyAsync(pulseId, 10, _cancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.OpinionThreads.Count(), Is.EqualTo(2));
                Assert.That(result.OpinionThreads.Any(ot => ot.OpinionName == "Agree"));
                Assert.That(result.OpinionThreads.Any(ot => ot.OpinionName == "Disagree"));
            });
        }

        [Test]
        public void VoteOnArgumentAsync_UserNotFound_ThrowsMissingDataException()
        {
            // Arrange
            var userId = "user1";
            var voteUpdateRequest = new ArgumentVoteUpdateRequest { ArgumentId = "argument1", VoteType = ArgumentVoteStatus.Upvote };
            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(userId, _cancellationToken)).ReturnsAsync((User?)null);

            // Act & Assert
            Assert.ThrowsAsync<MissingDataException>(() => _discussionHandler.VoteOnArgumentAsync(userId, voteUpdateRequest, _cancellationToken));
        }

        [TestCase(ArgumentVoteStatus.Upvote, ArgumentVoteStatus.Downvote, -1, 1)]
        [TestCase(ArgumentVoteStatus.Downvote, ArgumentVoteStatus.Upvote, 1, -1)]
        public async Task VoteOnArgumentAsync_SwitchingVottes_UpdatesVoteNumbersCorrectly(ArgumentVoteStatus currentVote, ArgumentVoteStatus newVote,
            int expectedUpvoteChange, int expectedDownvoteChange)
        {
            // Arrange
            var userId = "user1";

            var voteUpdateRequest = new ArgumentVoteUpdateRequest 
            {
                ArgumentId = "argument1",
                VoteType = newVote
            };

            var user = new User
            {
                Id = userId,
                ArgumentVotes = new List<ArgumentVote>
                {
                    new ArgumentVote
                    {
                        ArgumentId = "argument1",
                        VoteStatus = currentVote
                    }
                }.ToArray()
            };

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(userId, _cancellationToken)).ReturnsAsync(user);

            // Act
            await _discussionHandler.VoteOnArgumentAsync(userId, voteUpdateRequest, _cancellationToken);

            // Assert
            _mockArgumentRepository.Verify(cr => cr.AdjustArgumentVotesAsync("argument1", expectedUpvoteChange, expectedDownvoteChange, _cancellationToken), Times.Once);
            _mockUserRepository.Verify(ur => ur.UpdateArgumentVoteStatusAsync(userId, "argument1", newVote, _cancellationToken), Times.Once);
        }

        [Test]
        public async Task VoteOnArgumentAsync_NeutralVote_RemovesVote()
        {
            // Arrange
            var userId = "user1";

            var voteUpdateRequest = new ArgumentVoteUpdateRequest
            { 
                ArgumentId = "argument1",
                VoteType = ArgumentVoteStatus.Neutral
            };

            var user = new User
            {
                Id = userId,
                ArgumentVotes = new List<ArgumentVote> 
                { 
                    new ArgumentVote 
                    { ArgumentId = "argument1", 
                        VoteStatus = ArgumentVoteStatus.Upvote } 
                }.ToArray()
            };

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(userId, _cancellationToken)).ReturnsAsync(user);

            // Act
            await _discussionHandler.VoteOnArgumentAsync(userId, voteUpdateRequest, _cancellationToken);

            // Assert
            _mockArgumentRepository.Verify(cr => cr.AdjustArgumentVotesAsync("argument1", -1, 0, _cancellationToken), Times.Once);
            _mockUserRepository.Verify(ur => ur.RemoveArgumentVoteStatusAsync(userId, "argument1", _cancellationToken), Times.Once);
        }
    }
}

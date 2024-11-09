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
        private Mock<ICommentRepository> _mockCommentRepository;
        private Mock<IPulseRepository> _mockPulseRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private DiscussionHandler _discussionHandler;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockPulseRepository = new Mock<IPulseRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _discussionHandler = new DiscussionHandler(_mockCommentRepository.Object, _mockPulseRepository.Object, _mockUserRepository.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Test]
        public async Task CreateDiscussionCommentAsync_PulseExists_AddsComment()
        {
            // Arrange
            var discussionComment = new DiscussionComment { PulseId = "pulse1" };
            _mockPulseRepository.Setup(pr => pr.GetPulseAsync(discussionComment.PulseId)).ReturnsAsync(new Pulse());

            // Act
            await _discussionHandler.CreateDiscussionCommentAsync(discussionComment, _cancellationToken);

            // Assert
            _mockCommentRepository.Verify(cr => cr.AddCommentAsync(discussionComment, _cancellationToken), Times.Once);
        }

        [Test]
        public async Task CreateDiscussionCommentAsync_PulseDoesNotExist_DoesNotAddComment()
        {
            // Arrange
            var discussionComment = new DiscussionComment { PulseId = "pulse1" };
            _mockPulseRepository.Setup(pr => pr.GetPulseAsync(discussionComment.PulseId)).ReturnsAsync((Pulse?)null);

            // Act
            await _discussionHandler.CreateDiscussionCommentAsync(discussionComment, _cancellationToken);

            // Assert
            _mockCommentRepository.Verify(cr => cr.AddCommentAsync(It.IsAny<DiscussionComment>(), _cancellationToken), Times.Never);
        }

        [Test]
        public async Task GetDiscussionForPulseAsync_ReturnsGroupedCommentsByOpinion()
        {
            // Arrange
            var pulseId = "pulse1";
            var comments = new List<DiscussionComment>
            {
                new DiscussionComment { OpinionName = "Agree", CommentBody = "Comment 1" },
                new DiscussionComment { OpinionName = "Agree", CommentBody = "Comment 2" },
                new DiscussionComment { OpinionName = "Disagree", CommentBody = "Comment 3" }
            };
            _mockCommentRepository.Setup(cr => cr.GetCommentsForPulseIdAsync(pulseId, It.IsAny<int>(), _cancellationToken)).ReturnsAsync(comments);

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
        public void VoteOnCommentAsync_UserNotFound_ThrowsMissingDataException()
        {
            // Arrange
            var userId = "user1";
            var voteUpdateRequest = new CommentVoteUpdateRequest { CommentId = "comment1", VoteType = CommentVoteStatus.Upvote };
            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(userId, _cancellationToken)).ReturnsAsync((User?)null);

            // Act & Assert
            Assert.ThrowsAsync<MissingDataException>(() => _discussionHandler.VoteOnCommentAsync(userId, voteUpdateRequest, _cancellationToken));
        }

        [TestCase(CommentVoteStatus.Upvote, CommentVoteStatus.Downvote, -1, 1)]
        [TestCase(CommentVoteStatus.Downvote, CommentVoteStatus.Upvote, 1, -1)]
        public async Task VoteOnCommentAsync_SwitchingVottes_UpdatesVoteNumbersCorrectly(CommentVoteStatus currentVote, CommentVoteStatus newVote,
            int expectedUpvoteChange, int expectedDownvoteChange)
        {
            // Arrange
            var userId = "user1";

            var voteUpdateRequest = new CommentVoteUpdateRequest 
            {
                CommentId = "comment1",
                VoteType = newVote
            };

            var user = new User
            {
                Id = userId,
                CommentVotes = new List<CommentVote>
                {
                    new CommentVote
                    {
                        CommentId = "comment1",
                        VoteStatus = currentVote
                    }
                }.ToArray()
            };

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(userId, _cancellationToken)).ReturnsAsync(user);

            // Act
            await _discussionHandler.VoteOnCommentAsync(userId, voteUpdateRequest, _cancellationToken);

            // Assert
            _mockCommentRepository.Verify(cr => cr.AdjustCommentVotesAsync("comment1", expectedUpvoteChange, expectedDownvoteChange, _cancellationToken), Times.Once);
            _mockUserRepository.Verify(ur => ur.UpdateCommentVoteStatusAsync(userId, "comment1", newVote, _cancellationToken), Times.Once);
        }

        [Test]
        public async Task VoteOnCommentAsync_NeutralVote_RemovesVote()
        {
            // Arrange
            var userId = "user1";

            var voteUpdateRequest = new CommentVoteUpdateRequest
            { 
                CommentId = "comment1",
                VoteType = CommentVoteStatus.Neutral
            };

            var user = new User
            {
                Id = userId,
                CommentVotes = new List<CommentVote> 
                { 
                    new CommentVote 
                    { CommentId = "comment1", 
                        VoteStatus = CommentVoteStatus.Upvote } 
                }.ToArray()
            };

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(userId, _cancellationToken)).ReturnsAsync(user);

            // Act
            await _discussionHandler.VoteOnCommentAsync(userId, voteUpdateRequest, _cancellationToken);

            // Assert
            _mockCommentRepository.Verify(cr => cr.AdjustCommentVotesAsync("comment1", -1, 0, _cancellationToken), Times.Once);
            _mockUserRepository.Verify(ur => ur.RemoveCommentVoteStatusAsync(userId, "comment1", _cancellationToken), Times.Once);
        }
    }
}

using Moq;
using PulseService.Domain.Adapters;
using PulseService.Domain.Exceptions;
using PulseService.Domain.Handlers;
using PulseService.Domain.Models;

namespace PulseService.Domain.Tests.Unit.Handlers;

[TestFixture]
public class PulseHandlerTests
{
    private Mock<IPulseRepository> _mockPulseRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private PulseHandler _pulseHandler;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void Setup()
    {
        _mockPulseRepository = new Mock<IPulseRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _pulseHandler = new PulseHandler(_mockPulseRepository.Object, _mockUserRepository.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task CreatePulseAsync_AddsPulse()
    {
        // Arrange
        var pulse = new Pulse { Id = "pulse1", Title = "Test pulse" };

        // Act
        await _pulseHandler.CreatePulseAsync(pulse, _cancellationToken);

        // Assert
        _mockPulseRepository.Verify(pr => pr.AddPulseAsync(pulse, _cancellationToken), Times.Once);
    }

    [Test]
    public async Task DeletePulseAsync_PulseExists_ReturnsTrue()
    {
        // Arrange
        var pulseId = "pulse1";
        var userId = "user1";

        _mockPulseRepository.Setup(pr => pr.DeletePulseAsync(pulseId, userId, _cancellationToken)).ReturnsAsync(true);

        // Act
        var result = await _pulseHandler.DeletePulseAsync(pulseId, userId, _cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task DeletePulseAsync_PulseDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var pulseId = "pulse2";
        var userId = "user2";
        _mockPulseRepository.Setup(pr => pr.DeletePulseAsync(pulseId, userId, _cancellationToken)).ReturnsAsync(false);

        // Act
        var result = await _pulseHandler.DeletePulseAsync(pulseId, userId, _cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetAllPulsesAsync_ReturnsListOfPulses()
    {
        // Arrange
        var pulses = new List<Pulse>
        {
            new Pulse { Id = "pulse1", Title = "Title 1" },
            new Pulse { Id = "pulse2", Title = "Title 2" }
        };

        _mockPulseRepository.Setup(pr => pr.GetAllPulsesAsync(_cancellationToken)).ReturnsAsync(pulses);

        // Act
        var result = await _pulseHandler.GetAllPulsesAsync(_cancellationToken);

        // Assert
        Assert.That(result, Is.EquivalentTo(pulses));
    }

    [Test]
    public async Task GetPulseAsync_PulseExists_ReturnsPulse()
    {
        // Arrange
        var pulseId = "pulse1";
        var pulse = new Pulse { Id = pulseId, Title = "Test pulse" };
        _mockPulseRepository.Setup(pr => pr.GetPulseAsync(pulseId, _cancellationToken)).ReturnsAsync(pulse);

        // Act
        var result = await _pulseHandler.GetPulseAsync(pulseId, _cancellationToken);

        // Assert
        Assert.That(result, Is.EqualTo(pulse));
    }

    [Test]
    public void GetPulseAsync_PulseDoesNotExist_ThrowsMissingDataException()
    {
        // Arrange
        var pulseId = "missingPulse";
        _mockPulseRepository.Setup(pr => pr.GetPulseAsync(pulseId, _cancellationToken)).ReturnsAsync((Pulse?)null);

        // Act & Assert
        Assert.That(async () => await _pulseHandler.GetPulseAsync(pulseId, _cancellationToken),
            Throws.InstanceOf<MissingDataException>()
                .With.Message.Contains($"Failed to find Pulse with ID {pulseId}."));
    }

    [Test]
    public async Task UpdatePulseVoteAsync_CurrentVoteMatchesNewVote_DoesNotUpdate()
    {
        // Arrange
        var voteUpdate = new VoteUpdate { CurrentUserId = "user1", PulseId = "pulse1", VotedOpinion = "Agree" };
        var currentVote = new PulseVote { OpinionName = "Agree" };
        _mockUserRepository.Setup(ur => ur.GetCurrentPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId, _cancellationToken)).ReturnsAsync(currentVote);

        // Act
        await _pulseHandler.UpdatePulseVoteAsync(voteUpdate, _cancellationToken);

        // Assert
        _mockUserRepository.Verify(ur => ur.UpdatePulseVoteAsync(It.IsAny<VoteUpdate>(), _cancellationToken), Times.Never);
        _mockPulseRepository.Verify(pr => pr.UpdatePulseVoteAsync(It.IsAny<VoteUpdate>(), _cancellationToken), Times.Never);
    }

    [Test]
    public async Task UpdatePulseVoteAsync_CurrentVoteDiffersFromNewVote_UpdatesVote()
    {
        // Arrange
        var voteUpdate = new VoteUpdate { CurrentUserId = "user1", PulseId = "pulse1", VotedOpinion = "Disagree" };
        var currentVote = new PulseVote { OpinionName = "Agree" };
        _mockUserRepository.Setup(ur => ur.GetCurrentPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId, _cancellationToken)).ReturnsAsync(currentVote);

        _mockUserRepository.Setup(ur => ur.UpdatePulseVoteAsync(voteUpdate, _cancellationToken)).Returns(Task.CompletedTask);
        _mockPulseRepository.Setup(pr => pr.UpdatePulseVoteAsync(voteUpdate, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _pulseHandler.UpdatePulseVoteAsync(voteUpdate, _cancellationToken);

        // Assert
        _mockUserRepository.Verify(ur => ur.UpdatePulseVoteAsync(voteUpdate, _cancellationToken), Times.Once);
        _mockPulseRepository.Verify(pr => pr.UpdatePulseVoteAsync(voteUpdate, _cancellationToken), Times.Once);
    }
}
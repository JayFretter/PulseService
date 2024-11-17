using Moq;
using PulseService.Domain.Adapters;
using PulseService.Domain.Exceptions;
using PulseService.Domain.Handlers;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Tests.Unit.Handlers
{
    [TestFixture]
    public class ProfileHandlerTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IPulseRepository> _mockPulseRepository;
        private ProfileHandler _profileHandler;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPulseRepository = new Mock<IPulseRepository>();
            _profileHandler = new ProfileHandler(_mockPulseRepository.Object, _mockUserRepository.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Test]
        public async Task GetProfileByUsername_UserExists_ReturnsProfile()
        {
            // Arrange
            var username = "testuser";
            var userCredentials = new BasicUserCredentials
            {
                Id = "user1",
                Username = username,
                CreatedAtUtc = new DateTime(2020, 1, 1)
            };
            var pulses = new List<Pulse>
            {
                new Pulse { Id = "pulse1", Title = "Pulse 1" },
                new Pulse { Id = "pulse2", Title = "Pulse 2" }
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(username, _cancellationToken))
                .ReturnsAsync(userCredentials);

            _mockPulseRepository
                .Setup(repo => repo.GetPulsesByUserIdAsync(userCredentials.Id, _cancellationToken))
                .ReturnsAsync(pulses);

            // Act
            var profile = await _profileHandler.GetProfileByUsername(username, _cancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(profile, Is.Not.Null);
                Assert.That(profile.Username, Is.EqualTo(username));
                Assert.That(profile.UserSinceUtc, Is.EqualTo(userCredentials.CreatedAtUtc));
                Assert.That(profile.Pulses, Is.EquivalentTo(pulses));
            });
        }

        [Test]
        public void GetProfileByUsername_UserDoesNotExist_ThrowsMissingDataException()
        {
            // Arrange
            var username = "nonexistentuser";

            _mockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(username, _cancellationToken))
                .ReturnsAsync((BasicUserCredentials?)null);

            // Act & Assert
            Assert.That(
                async () => await _profileHandler.GetProfileByUsername(username, _cancellationToken),
                Throws.InstanceOf<MissingDataException>().With.Message.EqualTo($"User with username {username} not found.")
            );
        }

        [Test]
        public async Task GetProfileByUsername_NoPulses_ReturnsProfileWithEmptyPulses()
        {
            // Arrange
            var username = "testuser";
            var userCredentials = new BasicUserCredentials
            {
                Id = "user1",
                Username = username,
                CreatedAtUtc = new DateTime(2020, 1, 1)
            };
            
            var emptyPulses = new List<Pulse>();

            _mockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(username, _cancellationToken))
                .ReturnsAsync(userCredentials);

            _mockPulseRepository
                .Setup(repo => repo.GetPulsesByUserIdAsync(userCredentials.Id, _cancellationToken))
                .ReturnsAsync(emptyPulses);

            // Act
            var profile = await _profileHandler.GetProfileByUsername(username, _cancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(profile, Is.Not.Null);
                Assert.That(profile.Username, Is.EqualTo(username));
                Assert.That(profile.UserSinceUtc, Is.EqualTo(userCredentials.CreatedAtUtc));
                Assert.That(profile.Pulses, Is.Empty);
            });
        }
    }
}
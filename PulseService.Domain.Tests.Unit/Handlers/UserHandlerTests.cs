using Moq;
using PulseService.Domain.Adapters;
using PulseService.Domain.Handlers;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Tests.Unit.Handlers
{
    [TestFixture]
    public class UserHandlerTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private UserHandler _userHandler;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _userHandler = new UserHandler(_mockUserRepository.Object, _mockPasswordHasher.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Test]
        public async Task CreateUserAsync_HashesPasswordAndAddsUser()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "plaintextpassword" };
            var hashedPassword = "hashedpassword";
            _mockPasswordHasher.Setup(ph => ph.Hash(user.Password)).Returns(hashedPassword);

            // Act
            await _userHandler.CreateUserAsync(user);

            // Assert
            _mockUserRepository.Verify(ur => ur.AddUserAsync(It.Is<User>(u =>
                u.Username == "testuser" &&
                u.Password == hashedPassword
            )), Times.Once);
        }

        [Test]
        public async Task GetUserByUsernameAsync_UsernameExists_ReturnsCredentials()
        {
            // Arrange
            var username = "existingUser";
            var expectedCredentials = new BasicUserCredentials { Username = username };
            _mockUserRepository.Setup(ur => ur.GetUserByUsernameAsync(username, _cancellationToken)).ReturnsAsync(expectedCredentials);

            // Act
            var result = await _userHandler.GetUserByUsernameAsync(username, _cancellationToken);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCredentials));
        }

        [Test]
        public async Task GetUserByUsernameAsync_UsernameDoesNotExist_ReturnsNull()
        {
            // Arrange
            var username = "nonexistentUser";
            _mockUserRepository.Setup(ur => ur.GetUserByUsernameAsync(username, _cancellationToken)).ReturnsAsync((BasicUserCredentials?)null);

            // Act
            var result = await _userHandler.GetUserByUsernameAsync(username, _cancellationToken);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetUserByCredentialsAsync_HashesPasswordAndReturnsCredentials()
        {
            // Arrange
            var credentials = new UserCredentials { Username = "testuser", Password = "plaintextpassword" };
            var hashedPassword = "hashedpassword";
            var expectedCredentials = new BasicUserCredentials { Username = credentials.Username };

            _mockPasswordHasher.Setup(ph => ph.Hash(credentials.Password)).Returns(hashedPassword);
            _mockUserRepository
                .Setup(ur => ur.GetUserByCredentialsAsync(It.Is<UserCredentials>(c => c.Password == hashedPassword)))
                .ReturnsAsync(expectedCredentials);

            // Act
            var result = await _userHandler.GetUserByCredentialsAsync(credentials);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCredentials));
        }

        [Test]
        public async Task GetUserByCredentialsAsync_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var credentials = new UserCredentials { Username = "testuser", Password = "wrongpassword" };
            var hashedPassword = "hashedwrongpassword";
            _mockPasswordHasher.Setup(ph => ph.Hash(credentials.Password)).Returns(hashedPassword);
            _mockUserRepository.Setup(ur => ur.GetUserByCredentialsAsync(It.Is<UserCredentials>(c => c.Password == hashedPassword)))
                               .ReturnsAsync((BasicUserCredentials?)null);

            // Act
            var result = await _userHandler.GetUserByCredentialsAsync(credentials);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UsernameIsTakenAsync_UsernameExists_ReturnsTrue()
        {
            // Arrange
            var username = "existingUser";
            var basicUserCredentials = new BasicUserCredentials { Username = username };
            _mockUserRepository.Setup(ur => ur.GetUserByUsernameAsync(username, _cancellationToken)).ReturnsAsync(basicUserCredentials);

            // Act
            var result = await _userHandler.UsernameIsTakenAsync(username, _cancellationToken);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UsernameIsTakenAsync_UsernameDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var username = "nonexistentUser";
            _mockUserRepository.Setup(ur => ur.GetUserByUsernameAsync(username, _cancellationToken)).ReturnsAsync((BasicUserCredentials?)null);

            // Act
            var result = await _userHandler.UsernameIsTakenAsync(username, _cancellationToken);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
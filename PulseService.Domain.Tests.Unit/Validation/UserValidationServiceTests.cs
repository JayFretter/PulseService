using Moq;
using PulseService.Domain.Handlers;
using PulseService.Domain.Validation;

namespace PulseService.Domain.Tests.Unit.Validation;

public class UserValidationServiceTests
{
    private UserValidationService _validationService;
    private Mock<IUserHandler> _userHandlerMock;

    [SetUp]
    public void Setup()
    {
        _userHandlerMock = new Mock<IUserHandler>();
        _validationService = new UserValidationService(_userHandlerMock.Object);
    }

    [Test]
    public async Task GetValidationErrorsAsync_ReturnsError_WhenUsernameIsEmpty()
    {
        // Arrange
        var username = "";
        var password = "ValidPassword1!";
        _userHandlerMock.Setup(h => h.UsernameIsTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.GetValidationErrorsAsync(username, password, CancellationToken.None);

        // Assert
        Assert.That(result, Does.Contain("Username is required."));
    }

    [Test]
    public async Task GetValidationErrorsAsync_ReturnsError_WhenPasswordIsEmpty()
    {
        // Arrange
        var username = "ValidUsername";
        var password = "";
        _userHandlerMock.Setup(h => h.UsernameIsTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.GetValidationErrorsAsync(username, password, CancellationToken.None);

        // Assert
        Assert.That(result, Does.Contain("Password is required."));
    }

    [Test]
    public async Task GetValidationErrorsAsync_ReturnsError_WhenPasswordLacksSpecialCharacter()
    {
        // Arrange
        var username = "ValidUsername";
        var password = "ValidPassword123";
        _userHandlerMock.Setup(h => h.UsernameIsTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.GetValidationErrorsAsync(username, password, CancellationToken.None);

        // Assert
        Assert.That(result, Does.Contain("Password must contain at least one special character."));
    }

    [Test]
    public async Task GetValidationErrorsAsync_ReturnsError_WhenUsernameIsTaken()
    {
        // Arrange
        var username = "TakenUsername";
        var password = "ValidPassword1!";
        _userHandlerMock.Setup(h =>
                h.UsernameIsTakenAsync(It.Is<string>(u => u == username), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validationService.GetValidationErrorsAsync(username, password, CancellationToken.None);

        // Assert
        Assert.That(result, Does.Contain("Username is taken."));
    }

    [Test]
    public async Task GetValidationErrorsAsync_ReturnsNoError_WhenAllInputsAreValid()
    {
        // Arrange
        var username = "ValidUsername";
        var password = "ValidPassword1!";
        _userHandlerMock.Setup(h => h.UsernameIsTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.GetValidationErrorsAsync(username, password, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

}
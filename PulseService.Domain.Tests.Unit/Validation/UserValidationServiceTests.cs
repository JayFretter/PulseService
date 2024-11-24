using Microsoft.Extensions.Localization;
using Moq;
using PulseService.Domain.Handlers;
using PulseService.Domain.Validation;

namespace PulseService.Domain.Tests.Unit.Validation;

[TestFixture]
public class UserValidationServiceTests
{
    private UserValidationService _validationService;
    private Mock<IUserHandler> _userHandlerMock;
    private Mock<IStringLocalizer<UserValidationService>> _localizerMock;

    [SetUp]
    public void Setup()
    {
        _userHandlerMock = new Mock<IUserHandler>();
        _localizerMock = new Mock<IStringLocalizer<UserValidationService>>();

        // Setup mock localizer responses
        _localizerMock.Setup(l => l["UsernameRequired"])
            .Returns(new LocalizedString("UsernameRequired", "Username is required."));
        _localizerMock.Setup(l => l["PasswordRequired"])
            .Returns(new LocalizedString("PasswordRequired", "Password is required."));
        _localizerMock.Setup(l => l["PasswordSpecialCharacter"])
            .Returns(new LocalizedString("PasswordSpecialCharacter",
                "Password must contain at least one special character."));
        _localizerMock.Setup(l => l["UsernameTaken"])
            .Returns(new LocalizedString("UsernameTaken", "Username is taken."));

        _validationService = new UserValidationService(_userHandlerMock.Object, _localizerMock.Object);
    }

    [Test]
    public async Task GetValidationErrorsAsync_ReturnsError_WhenUsernameIsEmpty()
    {
        // Arrange
        string username = "";
        string password = "ValidPassword1!";
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
        string username = "ValidUsername";
        string password = "";
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
        string username = "ValidUsername";
        string password = "ValidPassword123";
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
        string username = "TakenUsername";
        string password = "ValidPassword1!";
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
        string username = "ValidUsername";
        string password = "ValidPassword1!";
        _userHandlerMock.Setup(h => h.UsernameIsTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validationService.GetValidationErrorsAsync(username, password, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
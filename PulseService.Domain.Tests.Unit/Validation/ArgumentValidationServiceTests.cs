using Microsoft.Extensions.Localization;
using Moq;
using PulseService.Domain.Models;
using PulseService.Domain.Validation;

namespace PulseService.Domain.Tests.Unit.Validation;

[TestFixture]
public class ArgumentValidationServiceTests
{
    private ArgumentValidationService _validationService;
    private Mock<IStringLocalizer<ArgumentValidationService>> _mockStringLocalizer;

    [SetUp]
    public void Setup()
    {
        _mockStringLocalizer = new Mock<IStringLocalizer<ArgumentValidationService>>();

        // Setup mock localizer responses
        _mockStringLocalizer.Setup(l => l["OpinionRequired"])
            .Returns(new LocalizedString("OpinionRequired", "An opinion must be selected to create an argument."));
        _mockStringLocalizer.Setup(l => l["ArgumentBodyRequired"])
            .Returns(new LocalizedString("ArgumentBodyRequired", "Argument cannot be empty."));
        _mockStringLocalizer.Setup(l => l["ArgumentBodyLength", It.IsAny<object[]>()])
            .Returns((string name, object[] args) =>
                new LocalizedString(name, $"Argument cannot be shorter than {args[0]} characters."));

        _validationService = new ArgumentValidationService(_mockStringLocalizer.Object);
    }

    [Test]
    public void GetValidationErrorsForNewArgument_ReturnsError_WhenOpinionNameIsEmpty()
    {
        // Arrange
        var argument = new DiscussionArgument
        {
            OpinionName = "",
            ArgumentBody = "This is a valid argument body that meets the length requirement."
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewArgument(argument);

        // Assert
        Assert.That(result, Does.Contain("An opinion must be selected to create an argument."));
    }

    [Test]
    public void GetValidationErrorsForNewArgument_ReturnsError_WhenArgumentBodyIsEmpty()
    {
        // Arrange
        var argument = new DiscussionArgument
        {
            OpinionName = "ValidOpinion",
            ArgumentBody = ""
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewArgument(argument);

        // Assert
        Assert.That(result, Does.Contain("Argument cannot be empty."));
    }

    [Test]
    public void GetValidationErrorsForNewArgument_ReturnsError_WhenArgumentBodyIsTooShort()
    {
        // Arrange
        var argument = new DiscussionArgument
        {
            OpinionName = "ValidOpinion",
            ArgumentBody = "Too short."
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewArgument(argument);

        // Assert
        Assert.That(result, Does.Contain("Argument cannot be shorter than 100 characters."));
    }

    [Test]
    public void GetValidationErrorsForNewArgument_ReturnsNoError_WhenAllInputsAreValid()
    {
        // Arrange
        var argument = new DiscussionArgument
        {
            OpinionName = "ValidOpinion",
            ArgumentBody = new string('A', 100) // Valid argument body of exactly 100 characters.
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewArgument(argument);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
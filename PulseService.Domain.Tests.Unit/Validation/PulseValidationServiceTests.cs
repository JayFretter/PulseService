using Microsoft.Extensions.Localization;
using Moq;
using PulseService.Domain.Models;
using PulseService.Domain.Validation;

namespace PulseService.Domain.Tests.Unit.Validation;

[TestFixture]
public class PulseValidationServiceTests
{
    private PulseValidationService _validationService;
    private Mock<IStringLocalizer<PulseValidationService>> _localizerMock;

    [SetUp]
    public void Setup()
    {
        _localizerMock = new Mock<IStringLocalizer<PulseValidationService>>();

        // Setup mock localizer responses
        _localizerMock.Setup(l => l["TitleRequired"])
            .Returns(new LocalizedString("TitleRequired", "Title is required."));
        _localizerMock.Setup(l => l["OpinionsCountRange", It.IsAny<object[]>()])
            .Returns((string name, object[] args) =>
                new LocalizedString(name,
                    $"The number of selectable opinions must be within the range {args[0]}-{args[1]}."));
        _localizerMock.Setup(l => l["OpinionsMustHaveName"])
            .Returns(new LocalizedString("OpinionsMustHaveName", "All selectable opinions must have a name."));
        _localizerMock.Setup(l => l["OpinionsMustBeUnique"])
            .Returns(new LocalizedString("OpinionsMustBeUnique", "All selectable opinions must be unique."));

        _validationService = new PulseValidationService(_localizerMock.Object);
    }

    [Test]
    public void GetValidationErrorsForNewPulse_ReturnsError_WhenTitleIsEmpty()
    {
        // Arrange
        var pulse = new Pulse
        {
            Title = "",
            Opinions = new List<Opinion>
            {
                new Opinion { Name = "Option 1" },
                new Opinion { Name = "Option 2" }
            }
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewPulse(pulse);

        // Assert
        Assert.That(result, Does.Contain("Title is required."));
    }

    [Test]
    public void GetValidationErrorsForNewPulse_ReturnsError_WhenOpinionsCountIsLessThanMin()
    {
        // Arrange
        var pulse = new Pulse
        {
            Title = "Valid Title",
            Opinions = new List<Opinion> { new Opinion { Name = "Option 1" } }
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewPulse(pulse);

        // Assert
        Assert.That(result, Does.Contain("The number of selectable opinions must be within the range 2-4."));
    }

    [Test]
    public void GetValidationErrorsForNewPulse_ReturnsError_WhenOpinionsCountIsGreaterThanMax()
    {
        // Arrange
        var pulse = new Pulse
        {
            Title = "Valid Title",
            Opinions = new List<Opinion>
            {
                new Opinion { Name = "Option 1" },
                new Opinion { Name = "Option 2" },
                new Opinion { Name = "Option 3" },
                new Opinion { Name = "Option 4" },
                new Opinion { Name = "Option 5" }
            }
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewPulse(pulse);

        // Assert
        Assert.That(result, Does.Contain("The number of selectable opinions must be within the range 2-4."));
    }

    [Test]
    public void GetValidationErrorsForNewPulse_ReturnsError_WhenAnOpinionNameIsEmpty()
    {
        // Arrange
        var pulse = new Pulse
        {
            Title = "Valid Title",
            Opinions = new List<Opinion>
            {
                new Opinion { Name = "Option 1" },
                new Opinion { Name = "" }
            }
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewPulse(pulse);

        // Assert
        Assert.That(result, Does.Contain("All selectable opinions must have a name."));
    }

    [Test]
    public void GetValidationErrorsForNewPulse_ReturnsError_WhenOpinionsAreNotUnique()
    {
        // Arrange
        var pulse = new Pulse
        {
            Title = "Valid Title",
            Opinions = new List<Opinion>
            {
                new Opinion { Name = "Option 1" },
                new Opinion { Name = "Option 1" }
            }
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewPulse(pulse);

        // Assert
        Assert.That(result, Does.Contain("All selectable opinions must be unique."));
    }

    [Test]
    public void GetValidationErrorsForNewPulse_ReturnsNoError_WhenAllInputsAreValid()
    {
        // Arrange
        var pulse = new Pulse
        {
            Title = "Valid Title",
            Opinions = new List<Opinion>
            {
                new Opinion { Name = "Option 1" },
                new Opinion { Name = "Option 2" }
            }
        };

        // Act
        var result = _validationService.GetValidationErrorsForNewPulse(pulse);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
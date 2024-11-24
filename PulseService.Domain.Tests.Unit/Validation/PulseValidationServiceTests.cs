using PulseService.Domain.Models;
using PulseService.Domain.Validation;

namespace PulseService.Domain.Tests.Unit.Validation;

[TestFixture]
public class PulseValidationServiceTests
{
    private PulseValidationService _validationService;

    [SetUp]
    public void Setup()
    {
        _validationService = new PulseValidationService();
    }

    [Test]
    public void GetValidationErrorsForNewPulse_ReturnsError_WhenTitleIsEmpty()
    {
        // Arrange
        var pulse = new Pulse
        {
            Title = "",
            Opinions = new List<Opinion> { new Opinion { Name = "Option 1" }, new Opinion { Name = "Option 2" } }
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
    public void GetValidationErrorsForNewPulse_ReturnsError_WhenOpinionNameIsEmpty()
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
    public void GetValidationErrorsForNewPulse_ReturnsNoError_WhenAllValid()
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
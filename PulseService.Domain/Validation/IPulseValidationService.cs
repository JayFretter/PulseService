using PulseService.Domain.Models;

namespace PulseService.Domain.Validation;

public interface IPulseValidationService
{
    public string[] GetValidationErrorsForNewPulse(Pulse pulse);
}
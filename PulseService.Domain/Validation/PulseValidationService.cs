using PulseService.Domain.Models;

namespace PulseService.Domain.Validation;

public class PulseValidationService : IPulseValidationService
{
    private const int MinOpinionCount = 2;
    private const int MaxOpinionCount = 4;
        
    public string[] GetValidationErrorsForNewPulse(Pulse pulse)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(pulse.Title))
        {
            validationErrors.Add("Title is required.");
        }
        if (pulse.Opinions.Count() < MinOpinionCount || pulse.Opinions.Count() > MaxOpinionCount)
        {
            validationErrors.Add($"The number of selectable opinions must be within the range {MinOpinionCount}-{MaxOpinionCount}.");
        }
        if (pulse.Opinions.Any(o => string.IsNullOrEmpty(o.Name)))
        {
            validationErrors.Add("All selectable opinions must have a name.");
        }

        if (pulse.Opinions.GroupBy(x => x.Name).Any(g => g.Count() > 1))
        {
            validationErrors.Add("All selectable opinions must be unique.");
        }
            
        return validationErrors.ToArray();
    }
}
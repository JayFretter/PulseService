using Microsoft.Extensions.Localization;
using PulseService.Domain.Models;

namespace PulseService.Domain.Validation;

public class PulseValidationService : IPulseValidationService
{
    private const int MinOpinionCount = 2;
    private const int MaxOpinionCount = 4;
    private readonly IStringLocalizer _localizer;

    public PulseValidationService(IStringLocalizer<PulseValidationService> localizer)
    {
        _localizer = localizer;
    }

    public string[] GetValidationErrorsForNewPulse(Pulse pulse)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(pulse.Title))
        {
            validationErrors.Add(_localizer["TitleRequired"]);
        }
        if (pulse.Opinions.Count() < MinOpinionCount || pulse.Opinions.Count() > MaxOpinionCount)
        {
            validationErrors.Add(_localizer["OpinionsCountRange", MinOpinionCount, MaxOpinionCount]);
        }
        if (pulse.Opinions.Any(o => string.IsNullOrEmpty(o.Name)))
        {
            validationErrors.Add(_localizer["OpinionsMustHaveName"]);
        }
        if (pulse.Opinions.GroupBy(x => x.Name).Any(g => g.Count() > 1))
        {
            validationErrors.Add(_localizer["OpinionsMustBeUnique"]);
        }

        return validationErrors.ToArray();
    }
}
using Microsoft.Extensions.Localization;
using PulseService.Domain.Models;

namespace PulseService.Domain.Validation;

public class ArgumentValidationService : IArgumentValidationService
{
    private const int MinArgumentLength = 100;
    private readonly IStringLocalizer _localizer;

    public ArgumentValidationService(IStringLocalizer<ArgumentValidationService> localizer)
    {
        _localizer = localizer;
    }

    public string[] GetValidationErrorsForNewArgument(DiscussionArgument argument)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(argument.OpinionName))
        {
            validationErrors.Add(_localizer["OpinionRequired"]);
        }
        if (string.IsNullOrWhiteSpace(argument.ArgumentBody))
        {
            validationErrors.Add(_localizer["ArgumentBodyRequired"]);
        }
        if (argument.ArgumentBody.Length < MinArgumentLength)
        {
            validationErrors.Add(_localizer["ArgumentBodyLength", MinArgumentLength]);
        }
            
        return validationErrors.ToArray();
    }
}
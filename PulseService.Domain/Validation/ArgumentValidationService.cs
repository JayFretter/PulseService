using PulseService.Domain.Models;

namespace PulseService.Domain.Validation;

public class ArgumentValidationService : IArgumentValidationService
{
    private const int MinArgumentLength = 100;
        
    public string[] GetValidationErrorsForNewArgument(DiscussionArgument argument)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(argument.OpinionName))
        {
            validationErrors.Add("An opinion must be selected to create an argument.");
        }
        if (string.IsNullOrWhiteSpace(argument.ArgumentBody))
        {
            validationErrors.Add("Argument cannot be empty.");
        }
        if (argument.ArgumentBody.Length < MinArgumentLength)
        {
            validationErrors.Add($"Argument cannot be shorter than {MinArgumentLength} characters.");
        }
            
        return validationErrors.ToArray();
    }
}
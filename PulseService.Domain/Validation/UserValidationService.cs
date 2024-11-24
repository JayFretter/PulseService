using Microsoft.Extensions.Localization;
using PulseService.Domain.Handlers;

namespace PulseService.Domain.Validation;

public class UserValidationService : IUserValidationService
{
    private readonly IUserHandler _userHandler;
    private readonly IStringLocalizer _localizer;

    public UserValidationService(IUserHandler userHandler, IStringLocalizer<UserValidationService> localizer)
    {
        _userHandler = userHandler;
        _localizer = localizer;
    }

    public async Task<string[]> GetValidationErrorsAsync(string username, string password, CancellationToken cancellationToken)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(username))
        {
            validationErrors.Add(_localizer["UsernameRequired"]);
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            validationErrors.Add(_localizer["PasswordRequired"]);
        }
        if (password.All(char.IsLetterOrDigit))
        {
            validationErrors.Add(_localizer["PasswordSpecialCharacter"]);
        }
        if (await _userHandler.UsernameIsTakenAsync(username, cancellationToken))
        {
            validationErrors.Add(_localizer["UsernameTaken"]);
        }
            
        return validationErrors.ToArray();
    }
}
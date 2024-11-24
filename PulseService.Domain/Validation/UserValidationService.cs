using PulseService.Domain.Handlers;

namespace PulseService.Domain.Validation;

public class UserValidationService : IUserValidationService
{
    private readonly IUserHandler _userHandler;

    public UserValidationService(IUserHandler userHandler)
    {
        _userHandler = userHandler;
    }

    public async Task<string[]> GetValidationErrorsAsync(string username, string password, CancellationToken cancellationToken)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(username))
        {
            validationErrors.Add("Username is required.");
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            validationErrors.Add("Password is required.");
        }
        if (password.All(char.IsLetterOrDigit))
        {
            validationErrors.Add("Password must contain at least one special character.");
        }
        if (await _userHandler.UsernameIsTakenAsync(username, cancellationToken))
        {
            validationErrors.Add("Username is taken.");
        }
            
        return validationErrors.ToArray();
    }
}
namespace PulseService.Domain.Validation;

public interface IUserValidationService
{
    public Task<string[]> GetValidationErrorsAsync(string username, string password, CancellationToken cancellationToken);
}
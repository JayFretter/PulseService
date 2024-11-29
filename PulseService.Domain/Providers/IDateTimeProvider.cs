namespace PulseService.Domain.Providers;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
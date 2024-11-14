using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public interface IProfileHandler
    {
        Task<Profile?> GetProfileByUsername(string username, CancellationToken cancellationToken);
    }
}
using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public interface IUserHandler
    {
        Task CreateUserAsync(User user);
    }
}

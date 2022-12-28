using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public interface IUserHandler
    {
        Task CreateUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UsernameIsTakenAsync(string username);
    }
}

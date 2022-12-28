using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Adapters
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}

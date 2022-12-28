using BiscuitService.Domain.Models;
using BiscuitService.Domain.Models.Dtos;

namespace BiscuitService.Domain.Handlers
{
    public interface IUserHandler
    {
        Task CreateUserAsync(User user);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<bool> UsernameIsTakenAsync(string username);
    }
}

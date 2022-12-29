using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Handlers
{
    public interface IUserHandler
    {
        Task CreateUserAsync(User user);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByCredentialsAsync(UserCredentials credentials);
        Task<bool> UsernameIsTakenAsync(string username);
    }
}

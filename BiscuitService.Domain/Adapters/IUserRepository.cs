using BiscuitService.Domain.Models;
using BiscuitService.Domain.Models.Dtos;

namespace BiscuitService.Domain.Adapters
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByCredentialsAsync(UserCredentials credentials);
    }
}

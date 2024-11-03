using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Handlers
{
    public interface IUserHandler
    {
        Task CreateUserAsync(User user);
        Task<BasicUserCredentials?> GetUserByUsernameAsync(string username);
        Task<BasicUserCredentials?> GetUserByCredentialsAsync(UserCredentials credentials);
        Task<bool> UsernameIsTakenAsync(string username);
    }
}

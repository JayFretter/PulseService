using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IUserRepository _userRepository;

        public UserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<bool> UsernameIsTakenAsync(string username)
        {
            if (await GetUserByUsernameAsync(username) is not null)
            {
                return true;
            }

            return false;
        }
    }
}

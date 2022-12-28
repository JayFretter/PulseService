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
    }
}

using PulseService.Domain.Adapters;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task CreateUserAsync(User user)
        {
            user.Password = _passwordHasher.Hash(user.Password);
            await _userRepository.AddUserAsync(user);
        }

        public async Task<BasicUserCredentials?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<BasicUserCredentials?> GetUserByCredentialsAsync(UserCredentials credentials)
        {
            credentials.Password = _passwordHasher.Hash(credentials.Password);
            return await _userRepository.GetUserByCredentialsAsync(credentials);
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

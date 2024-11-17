using PulseService.Domain.Adapters;
using PulseService.Domain.Exceptions;
using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public class ProfileHandler : IProfileHandler
    {
        private readonly IPulseRepository _pulseRepository;
        private readonly IUserRepository _userRepository;

        public ProfileHandler(IPulseRepository pulseRepository, IUserRepository userRepository)
        {
            _pulseRepository = pulseRepository;
            _userRepository = userRepository;
        }

        public async Task<Profile?> GetProfileByUsername(string username, CancellationToken cancellationToken)
        {
            var userCredentials = await _userRepository.GetUserByUsernameAsync(username, cancellationToken);
            if (userCredentials == null)
            {
                throw new MissingDataException($"User with username {username} not found.");
            }
            
            var pulsesByUser = await _pulseRepository.GetPulsesByUserIdAsync(userCredentials.Id, cancellationToken);

            return new Profile
            {
                Username = userCredentials.Username,
                UserSinceUtc = userCredentials.CreatedAtUtc,
                Pulses = pulsesByUser
            };
        }
    }
}
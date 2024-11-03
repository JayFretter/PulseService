using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Adapters
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByCredentialsAsync(UserCredentials credentials);
        Task UpdatePulseVoteAsync(VoteUpdate voteUpdate);
        Task<PulseVote?> GetCurrentPulseVote(string userId, string pulseId);
    }
}

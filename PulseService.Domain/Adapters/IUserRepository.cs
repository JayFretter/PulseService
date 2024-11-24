using PulseService.Domain.Enums;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Adapters
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
        Task<BasicUserCredentials?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<BasicUserCredentials?> GetUserByCredentialsAsync(UserCredentials credentials,
            CancellationToken cancellationToken);
        Task UpdatePulseVoteAsync(VoteUpdate voteUpdate, CancellationToken cancellationToken);
        Task<PulseVote?> GetCurrentPulseVote(string userId, string pulseId, CancellationToken cancellationToken);
        Task RemoveArgumentVoteStatusAsync(string userId, string argumentId, CancellationToken cancellationToken);
        Task UpdateArgumentVoteStatusAsync(string userId, string argumentId, ArgumentVoteStatus status, CancellationToken cancellationToken);
    }
}

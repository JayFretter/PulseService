using PulseService.Domain.Enums;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Adapters
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
        Task<BasicUserCredentials?> GetUserByUsernameAsync(string username);
        Task<BasicUserCredentials?> GetUserByCredentialsAsync(UserCredentials credentials);
        Task UpdatePulseVoteAsync(VoteUpdate voteUpdate, CancellationToken cancellationToken);
        Task<PulseVote?> GetCurrentPulseVote(string userId, string pulseId);
        Task RemoveCommentVoteStatusAsync(string userId, string commentId, CancellationToken cancellationToken);
        Task UpdateCommentVoteStatusAsync(string userId, string commentId, CommentVoteStatus status, CancellationToken cancellationToken);
    }
}

using BiscuitService.Domain.Models;
using BiscuitService.Domain.Models.Dtos;

namespace BiscuitService.Domain.Adapters
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByCredentialsAsync(UserCredentials credentials);

        /// <summary>
        /// Updates a user's vote on a Biscuit.
        /// </summary>
        /// <param name="voteUpdate"></param>
        /// <returns>The name of the previously voted option, if any.</returns>
        Task<string?> UpdateBiscuitVoteAsync(VoteUpdate voteUpdate);
    }
}

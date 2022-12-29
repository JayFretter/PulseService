using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public class BiscuitHandler : IBiscuitHandler
    {
        private readonly IBiscuitRepository _biscuitRepository;
        private readonly IUserRepository _userRepository;

        public BiscuitHandler(IBiscuitRepository biscuitRepository, IUserRepository userRepository)
        {
            _biscuitRepository = biscuitRepository;
            _userRepository = userRepository;
        }

        public async Task CreateBiscuitAsync(Biscuit biscuit)
        {
            await _biscuitRepository.AddBiscuitAsync(biscuit);
        }

        public async Task<bool> DeleteBiscuitAsync(string id, string currentUserId)
        {
            return await _biscuitRepository.DeleteBiscuitAsync(id, currentUserId);
        }

        public async Task<IEnumerable<Biscuit>> GetAllBiscuitsAsync()
        {
            return await _biscuitRepository.GetAllBiscuitsAsync();
        }

        public async Task UpdateBiscuitVoteAsync(VoteUpdate voteUpdate)
        {
            var currentVote = await _userRepository.GetCurrentBiscuitVote(voteUpdate.CurrentUserId, voteUpdate.BiscuitId);
            voteUpdate.UnvotedOpinion = currentVote?.OptionName;

            await _userRepository.UpdateBiscuitVoteAsync(voteUpdate);
            await _biscuitRepository.UpdateBiscuitVoteAsync(voteUpdate);
        }
    }
}

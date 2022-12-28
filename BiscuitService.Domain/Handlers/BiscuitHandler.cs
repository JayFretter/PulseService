using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public class BiscuitHandler : IBiscuitHandler
    {
        private readonly IBiscuitRepository _biscuitRepository;

        public BiscuitHandler(IBiscuitRepository biscuitRepository)
        {
            _biscuitRepository = biscuitRepository;
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

        public Task UpdateBiscuitVoteAsync(VoteUpdate voteUpdate)
        {
            throw new NotImplementedException();
        }
    }
}

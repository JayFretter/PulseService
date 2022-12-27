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

        public async Task DeleteBiscuitAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}

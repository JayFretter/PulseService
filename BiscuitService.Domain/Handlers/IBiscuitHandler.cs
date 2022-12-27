using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Handlers
{
    public interface IBiscuitHandler
    {
        public bool CreateBiscuit(Biscuit biscuit);
    }
}

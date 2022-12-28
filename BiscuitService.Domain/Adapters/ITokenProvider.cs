using BiscuitService.Domain.Models;

namespace BiscuitService.Domain.Adapters
{
    public interface ITokenProvider
    {
        string GenerateToken(User user);
    }
}

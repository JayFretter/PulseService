using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Adapters
{
    public interface ITokenManager
    {
        string GenerateToken(BasicUserCredentials user);
        BasicUserCredentials GetUserFromToken(string token);
    }
}

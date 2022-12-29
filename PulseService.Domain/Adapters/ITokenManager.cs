using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Adapters
{
    public interface ITokenManager
    {
        string GenerateToken(UserDto user);
        UserDto GetUserFromToken(string token);
    }
}

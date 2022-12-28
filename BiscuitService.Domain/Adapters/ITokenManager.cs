using BiscuitService.Domain.Models.Dtos;

namespace BiscuitService.Domain.Adapters
{
    public interface ITokenManager
    {
        string GenerateToken(UserDto user);
        UserDto GetUserFromToken(string token);
    }
}

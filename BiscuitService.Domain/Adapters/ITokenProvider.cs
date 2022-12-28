using BiscuitService.Domain.Models.Dtos;

namespace BiscuitService.Domain.Adapters
{
    public interface ITokenProvider
    {
        string GenerateToken(UserDto user);
        UserDto GetUserFromToken(string token);
    }
}

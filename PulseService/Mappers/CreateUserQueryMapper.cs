using BiscuitService.Domain.Models;
using BiscuitService.Models.Queries;

namespace BiscuitService.Mappers
{
    public static class CreateUserQueryMapper
    {
        public static User ToDomain(this CreateUserQuery query) 
        {
            return new User
            {
                Username = query.Username,
                Password = query.Password,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}

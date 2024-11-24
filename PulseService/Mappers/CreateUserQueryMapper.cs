using PulseService.Domain.Models;
using PulseService.Models.Queries;

namespace PulseService.Mappers;

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
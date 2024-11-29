using PulseService.Api.Models.Queries;
using PulseService.Domain.Models;

namespace PulseService.Api.Mappers;

public static class CreateUserQueryMapper
{
    public static User ToDomain(this CreateUserQuery query) 
    {
        return new User
        {
            Username = query.Username,
            Password = query.Password,
        };
    }
}
using PulseService.Api.Models.Queries;
using PulseService.Domain.Models;

namespace PulseService.Api.Mappers;

public static class LogInUserQueryMapper
{
    public static UserCredentials ToDomain(this LogInUserQuery query) 
    {
        return new UserCredentials
        {
            Username = query.Username,
            Password = query.Password,
        };
    }
}
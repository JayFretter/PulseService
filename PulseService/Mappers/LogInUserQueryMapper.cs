using PulseService.Domain.Models;
using PulseService.Models.Queries;

namespace PulseService.Mappers
{
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
}

using BiscuitService.Domain.Models;
using BiscuitService.Models.Queries;

namespace BiscuitService.Mappers
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

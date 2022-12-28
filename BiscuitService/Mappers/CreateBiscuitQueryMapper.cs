using BiscuitService.Domain.Models;
using BiscuitService.Domain.Models.Dtos;
using BiscuitService.Models.Queries;

namespace BiscuitService.Mappers
{
    public static class CreateBiscuitQueryMapper
    {
        public static Biscuit ToDomain(this CreateBiscuitQuery query, UserDto currentUser) 
        {
            return new Biscuit
            {
                Title = query.Title,
                Opinions = query.Opinions,
                CreatedBy = new BiscuitUserDetails
                {
                    Id = currentUser.Id,
                    Username = currentUser.Username
                },
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}

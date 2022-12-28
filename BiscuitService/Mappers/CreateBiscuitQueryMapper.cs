using BiscuitService.Domain.Models;
using BiscuitService.Models.Queries;

namespace BiscuitService.Mappers
{
    public static class CreateBiscuitQueryMapper
    {
        public static Biscuit ToDomain(this CreateBiscuitQuery query, string currentUserId) 
        {
            return new Biscuit
            {
                Title = query.Title,
                Opinions = query.Opinions,
                CreatedBy = currentUserId,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}

using BiscuitService.Domain.Models;
using BiscuitService.Domain.Models.Dtos;
using BiscuitService.Models.Queries;

namespace BiscuitService.Mappers
{
    public static class UpdateVoteQueryMapper
    {
        public static VoteUpdate ToDomain(this UpdateVoteQuery query, string currentUserId) 
        {
            return new VoteUpdate
            {
                BiscuitId = query.BiscuitId,
                OptionName = query.OptionName,
                CurrentUserId = currentUserId
            };
        }
    }
}

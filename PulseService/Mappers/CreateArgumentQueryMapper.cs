using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using PulseService.Models.Queries;

namespace PulseService.Mappers;

public static class CreateArgumentQueryMapper
{
    public static DiscussionArgument ToDomain(this CreateArgumentQuery query, BasicUserCredentials currentUser) 
    {
        return new DiscussionArgument
        {
            ParentArgumentId = query.ParentArgumentId,
            UserId = currentUser.Id,
            Username = currentUser.Username,
            OpinionName = query.OpinionName,
            ArgumentBody = query.OpinionBody,
            PulseId = query.PulseId,
            Upvotes = 0,
            Downvotes = 0
        };
    }
}
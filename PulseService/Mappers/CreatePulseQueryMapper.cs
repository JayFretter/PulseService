using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using PulseService.Models.Queries;

namespace PulseService.Mappers;

public static class CreatePulseQueryMapper
{
    public static Pulse ToDomain(this CreatePulseQuery query, BasicUserCredentials currentUser) 
    {
        return new Pulse
        {
            Title = query.Title,
            Tags = query.Tags ?? string.Empty,
            Opinions = query.Opinions,
            CreatedBy = new PulseUserDetails
            {
                Id = currentUser.Id,
                Username = currentUser.Username
            },
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
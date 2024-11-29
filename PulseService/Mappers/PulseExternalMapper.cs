using PulseService.Domain.Models;
using PulseService.Models;

namespace PulseService.Mappers;

public static class PulseExternalMapper
{
    public static PulseExternal ToExternal(this Pulse pulse)
    {
        return new PulseExternal
        {
            Id = pulse.Id!,
            Title = pulse.Title,
            Tags = pulse.Tags,
            Opinions = pulse.Opinions,
            CreatedBy = pulse.CreatedBy,
            CreatedAtUtc = pulse.CreatedAtUtc,
            UpdatedAtUtc = pulse.UpdatedAtUtc
        };
    }
}
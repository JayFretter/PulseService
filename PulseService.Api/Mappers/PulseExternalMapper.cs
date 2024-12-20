﻿using PulseService.Api.Models;
using PulseService.Domain.Models;

namespace PulseService.Api.Mappers;

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
﻿using PulseService.Domain.Models;
using PulseService.Models;
using PulseService.Models.Responses;

namespace PulseService.Mappers;

public static class GetAllPulsesResponseMapper
{
    public static IEnumerable<PulseExternal> FromDomain(this IEnumerable<Pulse> pulses) 
    {
        return pulses.Where(b => b.Id is not null).Select(b => b.FromDomain());
    }
}
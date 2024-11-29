using PulseService.Api.Models;
using PulseService.Domain.Models;
using PulseService.Api.Models.Responses;

namespace PulseService.Api.Mappers;

public static class GetAllPulsesResponseMapper
{
    public static IEnumerable<PulseExternal> ToExternal(this IEnumerable<Pulse> pulses) 
    {
        return pulses.Where(b => b.Id is not null).Select(b => b.ToExternal());
    }
}
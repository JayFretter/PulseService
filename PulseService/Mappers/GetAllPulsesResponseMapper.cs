using PulseService.Domain.Models;
using PulseService.Models;
using PulseService.Models.Responses;

namespace PulseService.Mappers
{
    public static class GetAllPulsesResponseMapper
    {
        public static GetAllPulsesResponse FromDomain(this IEnumerable<Pulse> pulses) 
        {
            return new GetAllPulsesResponse
            {
                Pulses = pulses.Where(b => b.Id is not null).Select(b =>
                    new PulseExternal
                    {
                        Id = b.Id!,
                        Title = b.Title,
                        Opinions = b.Opinions,
                        CreatedBy = b.CreatedBy,
                        CreatedAtUtc = b.CreatedAtUtc,
                        UpdatedAtUtc = b.UpdatedAtUtc
                    })
            };
        }
    }
}

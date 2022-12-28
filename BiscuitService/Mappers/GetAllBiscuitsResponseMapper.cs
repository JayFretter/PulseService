using BiscuitService.Domain.Models;
using BiscuitService.Models;
using BiscuitService.Models.Responses;

namespace BiscuitService.Mappers
{
    public static class GetAllBiscuitsResponseMapper
    {
        public static GetAllBiscuitsResponse FromDomain(this IEnumerable<Biscuit> biscuits) 
        {
            return new GetAllBiscuitsResponse
            {
                Biscuits = biscuits.Where(b => b.Id is not null).Select(b =>
                    new BiscuitExternal
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

using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Models;
using MongoDB.Bson;

namespace BiscuitService.DatabaseAdapter.Mongo.Mappers
{
    public static class BiscuitDboMapper
    {
        public static BiscuitDbo FromDomain(this Biscuit biscuit)
        {
            return new BiscuitDbo
            {
                Id = biscuit.Id ?? ObjectId.GenerateNewId().ToString(),
                Title = biscuit.Title,
                Opinions = biscuit.Opinions,
                CreatedAtUtc = biscuit.CreatedAtUtc,
                UpdatedAtUtc = biscuit.UpdatedAtUtc,
            };
        }

        public static Biscuit ToDomain(this BiscuitDbo dboBiscuit)
        {
            return new Biscuit
            {
                Id = dboBiscuit.Id,
                Title = dboBiscuit.Title,
                Opinions = dboBiscuit.Opinions,
                CreatedAtUtc = dboBiscuit.CreatedAtUtc,
                UpdatedAtUtc = dboBiscuit.UpdatedAtUtc
            };
        }
    }
}

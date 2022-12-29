using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Models;
using MongoDB.Bson;

namespace BiscuitService.DatabaseAdapter.Mongo.Mappers
{
    public static class BiscuitDocumentMapper
    {
        public static BiscuitDocument FromDomain(this Biscuit biscuit)
        {
            return new BiscuitDocument
            {
                Id = biscuit.Id ?? ObjectId.GenerateNewId().ToString(),
                Title = biscuit.Title,
                Opinions = biscuit.Opinions,
                CreatedBy = biscuit.CreatedBy,
                CreatedAtUtc = biscuit.CreatedAtUtc,
                UpdatedAtUtc = biscuit.UpdatedAtUtc,
            };
        }

        public static Biscuit ToDomain(this BiscuitDocument biscuitDoc)
        {
            return new Biscuit
            {
                Id = biscuitDoc.Id,
                Title = biscuitDoc.Title,
                Opinions = biscuitDoc.Opinions,
                CreatedBy = biscuitDoc.CreatedBy,
                CreatedAtUtc = biscuitDoc.CreatedAtUtc,
                UpdatedAtUtc = biscuitDoc.UpdatedAtUtc
            };
        }
    }
}

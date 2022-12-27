using BiscuitService.Domain.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BiscuitService.DatabaseAdapter.Mongo
{
    public class BiscuitDbo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();

        public static BiscuitDbo FromDomain(Biscuit domainBiscuit)
        {
            return new BiscuitDbo
            {
                Id = domainBiscuit.Id ?? ObjectId.GenerateNewId().ToString(),
                Title = domainBiscuit.Title,
                Opinions = domainBiscuit.Opinions
            };
        }

        public static Biscuit ToDomain(BiscuitDbo dboBiscuit)
        {
            return new Biscuit
            {
                Id = dboBiscuit.Id,
                Title = dboBiscuit.Title,
                Opinions = dboBiscuit.Opinions
            };
        }
    }
}

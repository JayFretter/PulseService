using BiscuitService.Domain.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BiscuitService.DatabaseAdapter.Mongo.Models
{
    public class BiscuitDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}

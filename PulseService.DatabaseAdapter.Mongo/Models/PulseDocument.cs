using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Models
{
    public class PulseDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
        public PulseUserDetails CreatedBy { get; set; } = new PulseUserDetails();
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}

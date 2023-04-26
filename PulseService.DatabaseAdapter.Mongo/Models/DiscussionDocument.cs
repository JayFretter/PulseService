using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Models
{
    public class DiscussionDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string PulseId { get; set; } = string.Empty;
        public IEnumerable<OpinionThread> OpinionThreads { get; set; } = new List<OpinionThread>();
    }
}

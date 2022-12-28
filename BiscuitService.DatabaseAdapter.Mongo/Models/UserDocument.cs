using BiscuitService.Domain.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BiscuitService.DatabaseAdapter.Mongo.Models
{
    public class UserDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public IEnumerable<Vote> Votes { get; set; } = new List<Vote>();
    }
}

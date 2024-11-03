using PulseService.Domain.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PulseService.DatabaseAdapter.Mongo.Models
{
    public class UserDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public PulseVote[] PulseVotes { get; set; } = Array.Empty<PulseVote>();
        public CommentVote[] CommentVotes { get; set; } = Array.Empty<CommentVote>();
    }
}

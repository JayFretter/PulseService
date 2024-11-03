using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PulseService.DatabaseAdapter.Mongo.Models
{
    public class CommentDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string PulseId { get; set; } = string.Empty;
        public string? ParentCommentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string OpinionName { get; set; } = string.Empty;
        public string CommentBody { get; set; } = string.Empty;
        public uint Upvotes { get; set; }
        public uint Downvotes { get; set; }
    }
}

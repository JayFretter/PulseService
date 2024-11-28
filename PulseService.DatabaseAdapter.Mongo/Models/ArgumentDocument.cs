using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PulseService.DatabaseAdapter.Mongo.Models;

public class ArgumentDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string PulseId { get; set; } = string.Empty;
    public string? ParentArgumentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string OpinionName { get; set; } = string.Empty;
    public string ArgumentBody { get; set; } = string.Empty;
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
}
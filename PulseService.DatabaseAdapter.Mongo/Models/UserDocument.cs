﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Models;

public class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public PulseVote[] PulseVotes { get; set; } = Array.Empty<PulseVote>();
    public ArgumentVote[] ArgumentVotes { get; set; } = Array.Empty<ArgumentVote>();
}
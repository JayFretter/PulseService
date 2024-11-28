using MongoDB.Bson;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Mappers;

public static class PulseDocumentMapper
{
    public static PulseDocument ToDocument(this Pulse pulse)
    {
        return new PulseDocument
        {
            Id = pulse.Id ?? ObjectId.GenerateNewId().ToString(),
            Title = pulse.Title,
            Tags = pulse.Tags,
            Opinions = pulse.Opinions,
            CreatedBy = pulse.CreatedBy,
            CreatedAtUtc = pulse.CreatedAtUtc,
            UpdatedAtUtc = pulse.UpdatedAtUtc,
        };
    }

    public static Pulse ToDomain(this PulseDocument pulseDoc)
    {
        return new Pulse
        {
            Id = pulseDoc.Id,
            Title = pulseDoc.Title,
            Tags = pulseDoc.Tags,
            Opinions = pulseDoc.Opinions,
            CreatedBy = pulseDoc.CreatedBy,
            CreatedAtUtc = pulseDoc.CreatedAtUtc,
            UpdatedAtUtc = pulseDoc.UpdatedAtUtc
        };
    }
}
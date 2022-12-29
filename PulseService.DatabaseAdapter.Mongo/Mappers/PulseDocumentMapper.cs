using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Models;
using MongoDB.Bson;

namespace PulseService.DatabaseAdapter.Mongo.Mappers
{
    public static class PulseDocumentMapper
    {
        public static PulseDocument FromDomain(this Pulse pulse)
        {
            return new PulseDocument
            {
                Id = pulse.Id ?? ObjectId.GenerateNewId().ToString(),
                Title = pulse.Title,
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
                Opinions = pulseDoc.Opinions,
                CreatedBy = pulseDoc.CreatedBy,
                CreatedAtUtc = pulseDoc.CreatedAtUtc,
                UpdatedAtUtc = pulseDoc.UpdatedAtUtc
            };
        }
    }
}

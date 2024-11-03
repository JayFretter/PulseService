using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PulseService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoPulseRepository : IPulseRepository
    {
        private readonly IMongoCollection<PulseDocument> _collection;
        public MongoPulseRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<PulseDocument>(mongoOptions.Value.PulseCollectionName);
        }

        public async Task AddPulseAsync(Pulse pulse)
        {
            var pulseDocument = pulse.FromDomain();
            await _collection.InsertOneAsync(pulseDocument);
        }

        public async Task<bool> DeletePulseAsync(string id, string currentUserId)
        {
            var result = await _collection.DeleteOneAsync(b =>
                b.Id == id &&
                b.CreatedBy.Id == currentUserId);

            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Pulse>> GetAllPulsesAsync()
        {
            var result = await _collection.FindAsync(_ => true);
            var pulseDocuments = result.ToList();

            var pulses = new List<Pulse>();
            foreach (var pulseDocument in pulseDocuments)
            {
                pulses.Add(pulseDocument.ToDomain());
            }

            return pulses;
        }

        public async Task<Pulse> GetPulseAsync(string id)
        {
            var result = await _collection.FindAsync(x => x.Id == id);
            var pulseDocument = result.FirstOrDefault();

            return pulseDocument.ToDomain();
        }

        public async Task UpdatePulseVoteAsync(VoteUpdate voteUpdate)
        {
            var filter = Builders<PulseDocument>.Filter.Eq(pulse => pulse.Id, voteUpdate.PulseId);
            var update = Builders<PulseDocument>.Update.Inc("Opinions.$[voted].Votes", 1).Inc("Opinions.$[unvoted].Votes", -1);

            var arrayFilters = new[]
            {
                new JsonArrayFilterDefinition<BsonDocument>(string.Format("{{\"voted.Name\": \"{0}\"}}", voteUpdate.VotedOpinion ?? string.Empty)),
                new JsonArrayFilterDefinition<BsonDocument>(string.Format("{{\"unvoted.Name\": \"{0}\"}}", voteUpdate.UnvotedOpinion ?? string.Empty))
            };

            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilters });
        }
    }
}

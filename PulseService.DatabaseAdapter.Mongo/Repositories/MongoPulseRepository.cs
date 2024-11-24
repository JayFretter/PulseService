using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoPulseRepository : IPulseRepository
    {
        private readonly IMongoCollection<PulseDocument> _collection;
        public MongoPulseRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<PulseDocument>(mongoOptions.Value.PulseCollectionName);
        }

        public async Task AddPulseAsync(Pulse pulse, CancellationToken cancellationToken)
        {
            var pulseDocument = pulse.FromDomain();
            await _collection.InsertOneAsync(pulseDocument, cancellationToken: cancellationToken);
        }

        public async Task<bool> DeletePulseAsync(string id, string currentUserId, CancellationToken cancellationToken)
        {
            var result = await _collection.DeleteOneAsync(b =>
                b.Id == id &&
                b.CreatedBy.Id == currentUserId, cancellationToken: cancellationToken);

            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Pulse>> GetAllPulsesAsync(CancellationToken cancellationToken)
        {
            var result = await _collection.FindAsync(_ => true, cancellationToken: cancellationToken);
            var pulseDocuments = result.ToList();

            return pulseDocuments.Select(pulseDocument => pulseDocument.ToDomain()).ToList();
        }

        public async Task<Pulse?> GetPulseAsync(string id, CancellationToken cancellationToken)
        {
            var result = await _collection.FindAsync(x => x.Id == id, cancellationToken: cancellationToken);
            var pulseDocument = result.FirstOrDefault();

            return pulseDocument?.ToDomain();
        }

        public async Task UpdatePulseVoteAsync(VoteUpdate voteUpdate, CancellationToken cancellationToken)
        {
            var filter = Builders<PulseDocument>.Filter.Eq(pulse => pulse.Id, voteUpdate.PulseId);
            var update = Builders<PulseDocument>.Update.Inc("Opinions.$[voted].Votes", 1).Inc("Opinions.$[unvoted].Votes", -1);

            var arrayFilters = new[]
            {
                new JsonArrayFilterDefinition<BsonDocument>(string.Format("{{\"voted.Name\": \"{0}\"}}", voteUpdate.VotedOpinion ?? string.Empty)),
                new JsonArrayFilterDefinition<BsonDocument>(string.Format("{{\"unvoted.Name\": \"{0}\"}}", voteUpdate.UnvotedOpinion ?? string.Empty))
            };

            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilters }, cancellationToken);
        }

        public async Task<IEnumerable<Pulse>> GetPulsesByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var filter = Builders<PulseDocument>.Filter.Eq(pulse => pulse.CreatedBy.Id, userId);

            var results = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return results.ToList(cancellationToken: cancellationToken).Select(r => r.ToDomain());
        }
    }
}

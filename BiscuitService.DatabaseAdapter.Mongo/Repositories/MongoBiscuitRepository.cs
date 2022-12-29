using BiscuitService.DatabaseAdapter.Mongo.Mappers;
using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BiscuitService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoBiscuitRepository : IBiscuitRepository
    {
        private readonly IMongoCollection<BiscuitDocument> _collection;
        public MongoBiscuitRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<BiscuitDocument>(mongoOptions.Value.BiscuitCollectionName);
        }

        public async Task AddBiscuitAsync(Biscuit biscuit)
        {
            var biscuitDocument = biscuit.FromDomain();
            await _collection.InsertOneAsync(biscuitDocument);
        }

        public async Task<bool> DeleteBiscuitAsync(string id, string currentUserId)
        {
            var result = await _collection.DeleteOneAsync(b =>
                b.Id == id &&
                b.CreatedBy.Id == currentUserId);

            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Biscuit>> GetAllBiscuitsAsync()
        {
            var result = await _collection.FindAsync(_ => true);
            var biscuitDocuments = result.ToList();

            var biscuits = new List<Biscuit>();
            foreach (var biscuitDocument in biscuitDocuments)
            {
                biscuits.Add(biscuitDocument.ToDomain());
            }

            return biscuits;
        }

        public async Task UpdateBiscuitVoteAsync(VoteUpdate voteUpdate)
        {
            if (voteUpdate.VotedOpinion.Equals(voteUpdate.UnvotedOpinion))
                return;

            var filter = Builders<BiscuitDocument>.Filter.Eq(b => b.Id, voteUpdate.BiscuitId);
            var update = Builders<BiscuitDocument>.Update.Inc("Opinions.$[voted].Votes", 1).Inc("Opinions.$[unvoted].Votes", -1);

            var arrayFilters = new[]
            {
                new JsonArrayFilterDefinition<BsonDocument>(string.Format("{{\"voted.Name\": \"{0}\"}}", voteUpdate.VotedOpinion)),
                new JsonArrayFilterDefinition<BsonDocument>(string.Format("{{\"unvoted.Name\": \"{0}\"}}", voteUpdate.UnvotedOpinion ?? string.Empty))
            };

            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilters });
        }
    }
}

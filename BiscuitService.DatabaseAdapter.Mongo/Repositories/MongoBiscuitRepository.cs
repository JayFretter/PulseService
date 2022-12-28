using BiscuitService.DatabaseAdapter.Mongo.Mappers;
using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;
using Microsoft.Extensions.Options;
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

        public async Task DeleteBiscuitAsync(string id)
        {
            await _collection.DeleteOneAsync(b => b.Id!.Equals(id));
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
    }
}

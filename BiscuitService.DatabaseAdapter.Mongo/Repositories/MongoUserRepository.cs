using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BiscuitService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<BiscuitDbo> _collection;
        public MongoUserRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<BiscuitDbo>(mongoOptions.Value.UserCollectionName);
        }

        public Task AddUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}

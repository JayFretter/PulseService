using BiscuitService.DatabaseAdapter.Mongo.Mappers;
using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BiscuitService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserDocument> _collection;
        public MongoUserRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<UserDocument>(mongoOptions.Value.UserCollectionName);
        }

        public async Task AddUserAsync(User user)
        {
            var userDocument = user.FromDomain();
            await _collection.InsertOneAsync(userDocument);
        }
    }
}

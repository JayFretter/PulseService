using BiscuitService.DatabaseAdapter.Mongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BiscuitService.DatabaseAdapter.Mongo
{
    public class MongoService
    {
        public MongoClient Client { get; set; }

        public MongoService(IOptions<MongoOptions> mongoOptions)
        {
            Client = new MongoClient(mongoOptions.Value.ConnectionString);
        }
    }
}

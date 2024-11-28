using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PulseService.DatabaseAdapter.Mongo.Models;

namespace PulseService.DatabaseAdapter.Mongo;

public class MongoService
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;

    public MongoService(IOptions<MongoOptions>? mongoOptions)
    {
        _client = new MongoClient(mongoOptions.Value.ConnectionString);
        _database = _client.GetDatabase(mongoOptions.Value.DatabaseName);
    }

    public IMongoDatabase GetDatabase()
    {
        return _database;
    }
}
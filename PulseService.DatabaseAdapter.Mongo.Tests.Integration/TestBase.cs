using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using PulseService.DatabaseAdapter.Mongo.Models;

namespace PulseService.DatabaseAdapter.Mongo.Tests.Integration;

public abstract class TestBase
{
    protected IOptions<MongoOptions> MongoOptions;
    protected MongoService MongoService;

    [OneTimeSetUp]
    protected void OneTimeSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings_testing.json")
            .Build();
        
        var mongoOptions = configuration.GetSection("MongoOptions").Get<MongoOptions>()!;
        MongoOptions = Options.Create(mongoOptions);
        
        MongoService = new MongoService(MongoOptions);
    }
}
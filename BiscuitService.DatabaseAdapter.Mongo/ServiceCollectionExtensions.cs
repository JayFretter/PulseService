using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.DatabaseAdapter.Mongo.Repositories;
using BiscuitService.Domain.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BiscuitService.DatabaseAdapter.Mongo
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoService(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<MongoOptions>(configuration.GetSection("MongoOptions"));
            services.AddSingleton<MongoService>();
            services.AddSingleton<IBiscuitRepository, MongoBiscuitRepository>();
            services.AddSingleton<IUserRepository, MongoUserRepository>();
        }
    }
}

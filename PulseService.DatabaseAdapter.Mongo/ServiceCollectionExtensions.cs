using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.DatabaseAdapter.Mongo.Repositories;
using PulseService.Domain.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PulseService.DatabaseAdapter.Mongo
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoService(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<MongoOptions>(configuration.GetSection("MongoOptions"));
            services.AddSingleton<MongoService>();
            services.AddSingleton<IPulseRepository, MongoPulseRepository>();
            services.AddSingleton<IUserRepository, MongoUserRepository>();
        }
    }
}

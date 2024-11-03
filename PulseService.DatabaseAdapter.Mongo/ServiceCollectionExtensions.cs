using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.DatabaseAdapter.Mongo.Repositories;
using PulseService.Domain.Adapters;

namespace PulseService.DatabaseAdapter.Mongo
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoService(this IServiceCollection services, IConfigurationRoot configuration)
        {
            return services
                .Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)))
                .AddSingleton<MongoService>()
                .AddSingleton<IPulseRepository, MongoPulseRepository>()
                .AddSingleton<IUserRepository, MongoUserRepository>()
                .AddSingleton<ICommentRepository, MongoCommentRepository>();
        }
    }
}

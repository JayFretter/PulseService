using BiscuitService.Domain.Adapters;

namespace BiscuitService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoUserRepository : IUserRepository
    {
        public MongoUserRepository(MongoService service)
        {
            var client = service.Client;
            var x = 2;
        }

        public string GetUsername()
        {
            var username = "steamboy80";
            return username;
        }
    }
}

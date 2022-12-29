namespace BiscuitService.DatabaseAdapter.Mongo.Models
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string BiscuitCollectionName { get; set; } = string.Empty;
        public string UserCollectionName { get; set; } = string.Empty;
    }
}

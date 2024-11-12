namespace PulseService.DatabaseAdapter.Mongo.Models
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string PulseCollectionName { get; set; } = string.Empty;
        public string UserCollectionName { get; set; } = string.Empty;
        public string DiscussionCollectionName { get; set; } = string.Empty;
        public string ArgumentCollectionName { get; set; } = string.Empty;
    }
}

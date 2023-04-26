using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Models;
using MongoDB.Bson;

namespace PulseService.DatabaseAdapter.Mongo.Mappers
{
    public static class DiscussionDocumentMapper
    {
        public static Discussion ToDomain(this DiscussionDocument discussionDocument)
        {
            return new Discussion
            {
                Id = discussionDocument.Id,
                PulseId = discussionDocument.PulseId,
                OpinionThreads = discussionDocument.OpinionThreads
            };
        }
    }
}

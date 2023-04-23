using PulseService.Domain.Adapters;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoDiscussionRepository : IDiscussionRepository
    {
        public Task AddDiscussionCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

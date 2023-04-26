using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoDiscussionRepository : IDiscussionRepository
    {
        private readonly IMongoCollection<DiscussionDocument> _collection;
        public MongoDiscussionRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<DiscussionDocument>(mongoOptions.Value.DiscussionCollectionName);
        }

        public async Task AddDiscussionCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken)
        {
            var result = await _collection.FindAsync(x => x.PulseId == discussionComment.PulseId);
            var existingDiscussion = result.FirstOrDefault();

            if (existingDiscussion is null) 
            {
                existingDiscussion = new DiscussionDocument
                {
                    PulseId = discussionComment.PulseId
                };
            }

            var existingOpinionThread = existingDiscussion.OpinionThreads.FirstOrDefault(t => t.ThreadOpinionName.Equals(discussionComment.OpinionName));
            if (existingOpinionThread is null) 
            {
                existingOpinionThread = new OpinionThread
                {
                    ThreadOpinionName = discussionComment.OpinionName,
                };
                existingDiscussion.OpinionThreads = existingDiscussion.OpinionThreads.Append(existingOpinionThread);
            }
            
            if (discussionComment.ParentCommentId is not null) 
            {
                var parentComment = existingOpinionThread.DiscussionComments.First(c => c.Id == discussionComment.ParentCommentId);
                parentComment.Children = parentComment.Children.Append(discussionComment);
            }
            else 
            {
                existingOpinionThread.DiscussionComments = existingOpinionThread.DiscussionComments.Append(discussionComment);
            }

            await _collection.ReplaceOneAsync(d => d.Id == existingDiscussion.Id, existingDiscussion, options: new ReplaceOptions { IsUpsert = true });
        }
    }
}

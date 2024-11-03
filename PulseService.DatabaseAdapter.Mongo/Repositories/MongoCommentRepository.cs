using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoCommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<CommentDocument> _collection;

        public MongoCommentRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<CommentDocument>(mongoOptions.Value.CommentCollectionName);
        }

        public async Task AddCommentAsync(DiscussionComment discussionComment, CancellationToken cancellationToken)
        {
            var commentDocument = new CommentDocument
            {
                PulseId = discussionComment.PulseId,
                ParentCommentId = discussionComment.ParentCommentId,
                UserId = discussionComment.UserId,
                Username = discussionComment.Username,
                OpinionName = discussionComment.OpinionName,
                CommentBody = discussionComment.CommentBody,
                Upvotes = discussionComment.Upvotes,
                Downvotes = discussionComment.Downvotes,
            };

            await _collection.InsertOneAsync(commentDocument, options: null, cancellationToken);
        }

        public async Task<IEnumerable<DiscussionComment>> GetCommentsForPulseIdAsync(string pulseId, int limit, CancellationToken cancellationToken)
        {
            var findOptions = new FindOptions<CommentDocument, CommentDocument>
            {
                Limit = limit
            };

            var result = await _collection.FindAsync(x => x.PulseId == pulseId, findOptions, cancellationToken);

            var comments = await result.ToListAsync(cancellationToken);
            if (comments == null)
            {
                return Array.Empty<DiscussionComment>();
            }

            return comments.Select(c => new DiscussionComment
            {
                Id = c.Id,
                ParentCommentId = c.ParentCommentId,
                UserId = c.UserId,
                Username = c.Username,
                OpinionName = c.OpinionName,
                CommentBody = c.CommentBody,
                PulseId = c.PulseId,
                Upvotes = c.Upvotes,
                Downvotes = c.Downvotes,
            });
        }

        public async Task<IEnumerable<DiscussionComment>> GetChildrenOfCommentIdAsync(string commentId, CancellationToken cancellationToken)
        {
            var result = await _collection.FindAsync(x => x.Id == commentId, cancellationToken: cancellationToken);

            var comments = await result.ToListAsync(cancellationToken);
            if (comments == null)
            {
                return Array.Empty<DiscussionComment>();
            }

            return comments.Select(c => new DiscussionComment
            {
                Id = c.Id,
                ParentCommentId = c.ParentCommentId,
                UserId = c.UserId,
                Username = c.Username,
                OpinionName = c.OpinionName,
                CommentBody = c.CommentBody,
                PulseId = c.PulseId,
                Upvotes = c.Upvotes,
                Downvotes = c.Downvotes,
            });
        }
        
        public async Task IncrementCommentUpvotesAsync(string commentId, int increment, CancellationToken cancellationToken)
        {
            var filter = Builders<CommentDocument>.Filter.Eq(c => c.Id, commentId);
            var update = Builders<CommentDocument>.Update.Inc("Upvotes", increment);

            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}

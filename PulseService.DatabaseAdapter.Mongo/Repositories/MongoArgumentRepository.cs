using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoArgumentRepository : IArgumentRepository
    {
        private readonly IMongoCollection<ArgumentDocument> _collection;

        public MongoArgumentRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<ArgumentDocument>(mongoOptions.Value.ArgumentCollectionName);
        }

        public async Task AddArgumentAsync(DiscussionArgument discussionArgument, CancellationToken cancellationToken)
        {
            var argumentDocument = new ArgumentDocument
            {
                PulseId = discussionArgument.PulseId,
                ParentArgumentId = discussionArgument.ParentArgumentId,
                UserId = discussionArgument.UserId,
                Username = discussionArgument.Username,
                OpinionName = discussionArgument.OpinionName,
                ArgumentBody = discussionArgument.ArgumentBody,
                Upvotes = discussionArgument.Upvotes,
                Downvotes = discussionArgument.Downvotes,
            };

            await _collection.InsertOneAsync(argumentDocument, options: null, cancellationToken);
        }

        public async Task<IEnumerable<DiscussionArgument>> GetArgumentsForPulseIdAsync(string pulseId, int limit, CancellationToken cancellationToken)
        {
            var findOptions = new FindOptions<ArgumentDocument, ArgumentDocument>
            {
                Limit = limit
            };

            var result = await _collection.FindAsync(x =>
                x.PulseId == pulseId &&
                x.ParentArgumentId == null,
                findOptions, cancellationToken);

            var arguments = await result.ToListAsync(cancellationToken);
            if (arguments == null)
            {
                return Array.Empty<DiscussionArgument>();
            }

            return arguments.Select(c => new DiscussionArgument
            {
                Id = c.Id,
                ParentArgumentId = c.ParentArgumentId,
                UserId = c.UserId,
                Username = c.Username,
                OpinionName = c.OpinionName,
                ArgumentBody = c.ArgumentBody,
                PulseId = c.PulseId,
                Upvotes = c.Upvotes,
                Downvotes = c.Downvotes,
            });
        }

        public async Task<IEnumerable<DiscussionArgument>> GetChildrenOfArgumentIdAsync(string argumentId, int limit, CancellationToken cancellationToken)
        {
            var findOptions = new FindOptions<ArgumentDocument, ArgumentDocument>
            {
                Limit = limit
            };
            
            var result = await _collection.FindAsync(x => x.ParentArgumentId == argumentId, findOptions, cancellationToken);

            var arguments = await result.ToListAsync(cancellationToken);
            if (arguments == null)
            {
                return Array.Empty<DiscussionArgument>();
            }

            return arguments.Select(c => new DiscussionArgument
            {
                Id = c.Id,
                ParentArgumentId = c.ParentArgumentId,
                UserId = c.UserId,
                Username = c.Username,
                OpinionName = c.OpinionName,
                ArgumentBody = c.ArgumentBody,
                PulseId = c.PulseId,
                Upvotes = c.Upvotes,
                Downvotes = c.Downvotes,
            });
        }
        
        public async Task IncrementArgumentUpvotesAsync(string argumentId, int increment, CancellationToken cancellationToken)
        {
            var filter = Builders<ArgumentDocument>.Filter.Eq(c => c.Id, argumentId);
            var update = Builders<ArgumentDocument>.Update.Inc("Upvotes", increment);

            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task IncrementArgumentDownvotesAsync(string argumentId, int increment, CancellationToken cancellationToken)
        {
            var filter = Builders<ArgumentDocument>.Filter.Eq(c => c.Id, argumentId);
            var update = Builders<ArgumentDocument>.Update.Inc("Downvotes", increment);

            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task AdjustArgumentVotesAsync(string argumentId, int upvoteIncrement, int downvoteIncrement, CancellationToken cancellationToken)
        {
            var filter = Builders<ArgumentDocument>.Filter.Eq(c => c.Id, argumentId);
            var update = Builders<ArgumentDocument>.Update.Inc("Upvotes", upvoteIncrement).Inc("Downvotes", downvoteIncrement);

            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}

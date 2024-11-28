using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Repositories;

public class MongoArgumentRepository : IArgumentRepository
{
    private readonly IMongoCollection<ArgumentDocument> _collection;

    public MongoArgumentRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
    {
        _collection = service.GetDatabase().GetCollection<ArgumentDocument>(mongoOptions.Value.ArgumentCollectionName);
    }

    public async Task AddArgumentAsync(DiscussionArgument discussionArgument, CancellationToken cancellationToken)
    {
        var argumentDocument = discussionArgument.ToDocument();
        await _collection.InsertOneAsync(argumentDocument, options: null, cancellationToken);
    }

    public async Task<IEnumerable<DiscussionArgument>> GetTopLevelArgumentsForPulseIdAsync(string pulseId, int limit, CancellationToken cancellationToken)
    {
        var findOptions = new FindOptions<ArgumentDocument, ArgumentDocument>
        {
            Limit = limit
        };

        var result = await _collection.FindAsync(a =>
                a.PulseId == pulseId &&
                a.ParentArgumentId == null,
            findOptions, cancellationToken);

        var arguments = await result.ToListAsync(cancellationToken);
        if (arguments == null)
        {
            return Array.Empty<DiscussionArgument>();
        }

        return arguments.Select(c => c.ToDomain());
    }

    public async Task<IEnumerable<DiscussionArgument>> GetChildrenOfArgumentIdAsync(string argumentId, int limit, CancellationToken cancellationToken)
    {
        var findOptions = new FindOptions<ArgumentDocument, ArgumentDocument>
        {
            Limit = limit
        };
            
        var result = await _collection.FindAsync(a => a.ParentArgumentId == argumentId, findOptions, cancellationToken);

        var arguments = await result.ToListAsync(cancellationToken);
        if (arguments == null)
        {
            return Array.Empty<DiscussionArgument>();
        }

        return arguments.Select(c => c.ToDomain());
    }

    public async Task AdjustArgumentVotesAsync(string argumentId, int upvoteIncrement, int downvoteIncrement, CancellationToken cancellationToken)
    {
        var filter = Builders<ArgumentDocument>.Filter.Eq(a => a.Id, argumentId);
        var update = Builders<ArgumentDocument>.Update.Inc("Upvotes", upvoteIncrement).Inc("Downvotes", downvoteIncrement);

        await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    public async Task SetArgumentToDeletedAsync(string userId, string argumentId, CancellationToken cancellationToken)
    {
        var filter = Builders<ArgumentDocument>.Filter.Where(a => a.UserId == userId && a.Id == argumentId);
            
        var update = Builders<ArgumentDocument>.Update
            .Set("Username", "[deleted]")
            .Set("ArgumentBody", "[deleted]");
            
        await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}
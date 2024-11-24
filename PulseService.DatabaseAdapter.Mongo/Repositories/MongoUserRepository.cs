using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.DatabaseAdapter.Mongo.Repositories;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection;
    public MongoUserRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
    {
        _collection = service.GetDatabase().GetCollection<UserDocument>(mongoOptions.Value.UserCollectionName);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        var userDocument = user.FromDomain();
        await _collection.InsertOneAsync(userDocument, cancellationToken: cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var matchingUsers = await _collection.FindAsync(u => u.Id == userId, cancellationToken: cancellationToken);
        return matchingUsers.FirstOrDefault(cancellationToken: cancellationToken)?.ToDomain();
    }

    public async Task<BasicUserCredentials?> GetUserByUsernameAsync(string username,
        CancellationToken cancellationToken)
    {
        var result = await _collection.FindAsync(u => u.Username.ToLower() == username.ToLower(), cancellationToken: cancellationToken);

        var userDocument = result.FirstOrDefault(cancellationToken: cancellationToken);
        return userDocument?.ToDto();
    }

    public async Task<BasicUserCredentials?> GetUserByCredentialsAsync(UserCredentials credentials,
        CancellationToken cancellationToken)
    {
        var result = await _collection.FindAsync(u => 
            u.Username.ToLower() == credentials.Username.ToLower() &&
            u.Password == credentials.Password, cancellationToken: cancellationToken);

        var userDocument = result.FirstOrDefault();
        if (userDocument is not null)
        {
            return userDocument.ToDto();
        }

        return null;
    }

    public async Task UpdatePulseVoteAsync(VoteUpdate voteUpdate, CancellationToken cancellationToken)
    {
        if (voteUpdate.VotedOpinion is null)
        {
            await DeleteCurrentPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId, cancellationToken);
            return;
        }

        var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, voteUpdate.CurrentUserId);

        var filterForExistingVote = filterForCurrentUser
                                    & Builders<UserDocument>.Filter.ElemMatch(u => u.PulseVotes, Builders<PulseVote>.Filter.Eq(v => v.PulseId, voteUpdate.PulseId));

        var update = Builders<UserDocument>.Update.Set(u => u.PulseVotes[-1].OpinionName, voteUpdate.VotedOpinion);
            
        var updateResult = await _collection.UpdateOneAsync(filterForExistingVote, update, cancellationToken: cancellationToken);

        // If an existing vote could not be found, add a new pulse vote
        if (updateResult.ModifiedCount == 0)
        {
            await AddNewPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId, voteUpdate.VotedOpinion, cancellationToken);
        }
    }

    public async Task<PulseVote?> GetCurrentPulseVoteAsync(string userId, string pulseId,
        CancellationToken cancellationToken)
    {
        var userDocument = (await _collection.FindAsync(u => u.Id == userId, cancellationToken: cancellationToken)).First();

        return userDocument.PulseVotes.FirstOrDefault(v => v.PulseId == pulseId);
    }

    public async Task UpdateArgumentVoteStatusAsync(string userId, string argumentId, ArgumentVoteStatus status, CancellationToken cancellationToken)
    {

        var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserDocument>.Update.Set("ArgumentVotes.$[voted].VoteStatus", (int)status);

        var arrayFilters = new[]
        {
            new JsonArrayFilterDefinition<BsonDocument>(string.Format("{{\"voted.ArgumentId\": \"{0}\"}}", argumentId)),
        };

        var updateResult = await _collection.UpdateOneAsync(filterForCurrentUser, update, new UpdateOptions { ArrayFilters = arrayFilters }, cancellationToken);

        if (updateResult.ModifiedCount == 0 && updateResult.MatchedCount != 0)
        {
            await AddArgumentVoteStatusAsync(userId, argumentId, status, cancellationToken);
        }
    }

    public async Task RemoveArgumentVoteStatusAsync(string userId, string argumentId, CancellationToken cancellationToken)
    {
        var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserDocument>.Update.PullFilter(u => u.ArgumentVotes, cv => cv.ArgumentId == argumentId);

        await _collection.UpdateOneAsync(filterForCurrentUser, update, cancellationToken: cancellationToken);
    }

    private async Task AddArgumentVoteStatusAsync(string userId, string argumentId, ArgumentVoteStatus status, CancellationToken cancellationToken)
    {
        var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserDocument>.Update.Push(
            u => u.ArgumentVotes,
            new ArgumentVote
            {
                ArgumentId = argumentId,
                VoteStatus = status,
            });

        await _collection.UpdateOneAsync(filterForCurrentUser, update, cancellationToken: cancellationToken);
    }

    private async Task DeleteCurrentPulseVote(string userId, string pulseId, CancellationToken cancellationToken)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserDocument>.Update.PullFilter(u => u.PulseVotes, v => v.PulseId == pulseId);

        await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
        
    private async Task AddNewPulseVote(string userId, string pulseId, string opinionName, CancellationToken cancellationToken)
    {
        var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserDocument>.Update.Push(
            u => u.PulseVotes,
            new PulseVote
            {
                PulseId = pulseId,
                OpinionName = opinionName
            });

        await _collection.UpdateOneAsync(filterForCurrentUser, update, cancellationToken: cancellationToken);
    }
}
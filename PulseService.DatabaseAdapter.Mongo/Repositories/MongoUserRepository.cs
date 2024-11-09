using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Enums;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserDocument> _collection;
        public MongoUserRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<UserDocument>(mongoOptions.Value.UserCollectionName);
        }

        public async Task AddUserAsync(User user)
        {
            var userDocument = user.FromDomain();
            await _collection.InsertOneAsync(userDocument);
        }

        public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var matchingUsers = await _collection.FindAsync(u => u.Id == userId);
            return matchingUsers.FirstOrDefault()?.ToDomain();
        }

        public async Task<BasicUserCredentials?> GetUserByUsernameAsync(string username)
        {
            var result = await _collection.FindAsync(u => u.Username.ToLower() == username.ToLower());

            var userDocument = result.FirstOrDefault();
            if (userDocument is not null)
            {
                return userDocument.ToDto();
            }

            return null;
        }

        public async Task<BasicUserCredentials?> GetUserByCredentialsAsync(UserCredentials credentials)
        {
            var result = await _collection.FindAsync(u => 
                u.Username.ToLower() == credentials.Username.ToLower() &&
                u.Password == credentials.Password);

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
            
            var updateResult = await _collection.UpdateOneAsync(filterForExistingVote, update);

            // If an existing vote could not be found, add a new pulse vote
            if (updateResult.ModifiedCount == 0)
            {
                await AddNewPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId, voteUpdate.VotedOpinion, cancellationToken);
            }
        }

        public async Task<PulseVote?> GetCurrentPulseVote(string userId, string pulseId)
        {
            var userDocument = (await _collection.FindAsync(u => u.Id == userId)).First();

            return userDocument.PulseVotes.FirstOrDefault(v => v.PulseId == pulseId);
        }

        public async Task UpdateCommentVoteStatusAsync(string userId, string commentId, CommentVoteStatus status, CancellationToken cancellationToken)
        {

            var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UserDocument>.Update.Set("CommentVotes.$[voted].VoteStatus", (int)status);

            var arrayFilters = new[]
            {
                new JsonArrayFilterDefinition<BsonDocument>(string.Format("{{\"voted.CommentId\": \"{0}\"}}", commentId)),
            };

            var updateResult = await _collection.UpdateOneAsync(filterForCurrentUser, update, new UpdateOptions { ArrayFilters = arrayFilters }, cancellationToken);

            if (updateResult.ModifiedCount == 0 && updateResult.MatchedCount != 0)
            {
                await AddCommentVoteStatusAsync(userId, commentId, status, cancellationToken);
            }
        }

        public async Task RemoveCommentVoteStatusAsync(string userId, string commentId, CancellationToken cancellationToken)
        {
            var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UserDocument>.Update.PullFilter(u => u.CommentVotes, cv => cv.CommentId == commentId);

            await _collection.UpdateOneAsync(filterForCurrentUser, update, cancellationToken: cancellationToken);
        }

        private async Task AddCommentVoteStatusAsync(string userId, string commentId, CommentVoteStatus status, CancellationToken cancellationToken)
        {
            var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UserDocument>.Update.Push(
                u => u.CommentVotes,
                new CommentVote
                {
                    CommentId = commentId,
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
}

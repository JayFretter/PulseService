using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Adapters;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Net.NetworkInformation;
using Microsoft.VisualBasic;

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

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var result = await _collection.FindAsync(u => u.Username.ToLower() == username.ToLower());

            var userDocument = result.FirstOrDefault();
            if (userDocument is not null)
            {
                return userDocument.ToDto();
            }

            return null;
        }

        public async Task<UserDto?> GetUserByCredentialsAsync(UserCredentials credentials)
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

        public async Task UpdatePulseVoteAsync(VoteUpdate voteUpdate)
        {
            if (voteUpdate.VotedOpinion is null)
            {
                await DeleteCurrentPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId);
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
                await AddNewPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId, voteUpdate.VotedOpinion);
            }
        }

        public async Task<PulseVote?> GetCurrentPulseVote(string userId, string pulseId)
        {
            var userDocument = (await _collection.FindAsync(u => u.Id == userId)).First();

            return userDocument.PulseVotes.FirstOrDefault(v => v.PulseId == pulseId);
        }

        private async Task DeleteCurrentPulseVote(string userId, string pulseId)
        {
            var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UserDocument>.Update.PullFilter(u => u.PulseVotes, v => v.PulseId == pulseId);

            await _collection.UpdateOneAsync(filter, update);
        }
        
        private async Task AddNewPulseVote(string userId, string pulseId, string opinionName)
        {
            var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UserDocument>.Update.Push(
                    u => u.PulseVotes,
                    new PulseVote
                    {
                        PulseId = pulseId,
                        OpinionName = opinionName
                    });

            await _collection.UpdateOneAsync(filterForCurrentUser, update);
        }
    }
}

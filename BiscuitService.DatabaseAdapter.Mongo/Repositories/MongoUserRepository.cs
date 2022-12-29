using BiscuitService.DatabaseAdapter.Mongo.Mappers;
using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;
using BiscuitService.Domain.Models.Dtos;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Net.NetworkInformation;

namespace BiscuitService.DatabaseAdapter.Mongo.Repositories
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

        public async Task UpdateBiscuitVoteAsync(VoteUpdate voteUpdate)
        {
            var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, voteUpdate.CurrentUserId);
            UpdateDefinition<UserDocument> update;

            var previousVote = await GetCurrentBiscuitVote(voteUpdate.CurrentUserId, voteUpdate.BiscuitId);
            if (previousVote is not null)
            {
                filter &= Builders<UserDocument>.Filter.Eq("Votes.BiscuitId", voteUpdate.BiscuitId);
                update = Builders<UserDocument>.Update.Set("Votes.$.OptionName", voteUpdate.VotedOpinion);
            }
            else
            {
                update = Builders<UserDocument>.Update.Push(u => u.Votes,
                    new Vote
                    {
                        BiscuitId = voteUpdate.BiscuitId,
                        OptionName = voteUpdate.VotedOpinion
                    });
            }

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<Vote?> GetCurrentBiscuitVote(string userId, string biscuitId)
        {
            var userDocument = (await _collection.FindAsync(u => u.Id == userId)).First();

            return userDocument.Votes.FirstOrDefault(v => v.BiscuitId == biscuitId);
        }
    }
}

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
            var filterForCurrentUser = Builders<UserDocument>.Filter.Eq(u => u.Id, voteUpdate.CurrentUserId);

            var filterForExistingVote = filterForCurrentUser
                & Builders<UserDocument>.Filter.ElemMatch(u => u.Votes, Builders<Vote>.Filter.Eq(v => v.BiscuitId, voteUpdate.BiscuitId));

            var update = Builders<UserDocument>.Update.Set(u => u.Votes[-1].OptionName, voteUpdate.VotedOpinion);
            
            var updateResult = await _collection.UpdateOneAsync(filterForExistingVote, update);

            // ModifiedCount will be 0 if an existing vote could not be found (and thus was not updated)
            if (updateResult.ModifiedCount == 0)
            {
                update = Builders<UserDocument>.Update.Push(
                    u => u.Votes,
                    new Vote
                    {
                        BiscuitId = voteUpdate.BiscuitId,
                        OptionName = voteUpdate.VotedOpinion
                    });

                await _collection.UpdateOneAsync(filterForCurrentUser, update);
            }
        }

        public async Task<Vote?> GetCurrentBiscuitVote(string userId, string biscuitId)
        {
            var userDocument = (await _collection.FindAsync(u => u.Id == userId)).First();

            return userDocument.Votes.FirstOrDefault(v => v.BiscuitId == biscuitId);
        }
    }
}

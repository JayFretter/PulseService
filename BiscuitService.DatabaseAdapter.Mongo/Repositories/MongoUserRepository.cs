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

        public async Task<string?> UpdateBiscuitVoteAsync(VoteUpdate voteUpdate)
        {
            var userDocument = (await _collection.FindAsync(u => u.Id == voteUpdate.CurrentUser.Id)).First();
            var previousVote = userDocument.Votes.FirstOrDefault(v => v.BiscuitId == voteUpdate.BiscuitId);

            FilterDefinition<UserDocument> filter;
            UpdateDefinition<UserDocument> update;
            string? previousVoteOption = null;

            if (previousVote != null)
            {
                previousVoteOption = previousVote.OptionName;

                filter = Builders<UserDocument>.Filter.Eq(u => u.Id, voteUpdate.CurrentUser.Id)
                & Builders<UserDocument>.Filter.Eq("Votes.BiscuitId", voteUpdate.BiscuitId);

                update = Builders<UserDocument>.Update.Set("Votes.$.OptionName", voteUpdate.OptionName);
            }
            else
            {
                filter = Builders<UserDocument>.Filter.Eq(u => u.Id, voteUpdate.CurrentUser.Id);

                update = Builders<UserDocument>.Update.Push(u => u.Votes,
                    new Vote
                    {
                        BiscuitId = voteUpdate.BiscuitId,
                        OptionName = voteUpdate.OptionName
                    });
            }

            await _collection.UpdateOneAsync(filter, update);

            return previousVoteOption;
        }
    }
}

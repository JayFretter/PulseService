using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Models;
using MongoDB.Bson;

namespace BiscuitService.DatabaseAdapter.Mongo.Mappers
{
    public static class UserDocumentMapper
    {
        public static UserDocument FromDomain(this User user)
        {
            return new UserDocument
            {
                Id = user.Id ?? ObjectId.GenerateNewId().ToString(),
                Username = user.Username,
                Password = user.Password,
                CreatedAtUtc = user.CreatedAtUtc,
                Votes = user.Votes
            };
        }

        public static User ToDomain(this UserDocument userDoc)
        {
            return new User
            {
                Id = userDoc.Id,
                Username = userDoc.Username,
                Password = userDoc.Password,
                CreatedAtUtc = userDoc.CreatedAtUtc,
                Votes = userDoc.Votes
            };
        }
    }
}

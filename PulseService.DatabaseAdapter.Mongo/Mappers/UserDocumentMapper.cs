using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using MongoDB.Bson;

namespace PulseService.DatabaseAdapter.Mongo.Mappers
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
                PulseVotes = user.PulseVotes,
                ArgumentVotes = user.ArgumentVotes
            };
        }
        
        public static User ToDomain(this UserDocument userDocument)
        {
            return new User
            {
                Id = userDocument.Id,
                Username = userDocument.Username,
                Password = userDocument.Password,
                CreatedAtUtc = userDocument.CreatedAtUtc,
                PulseVotes = userDocument.PulseVotes,
                ArgumentVotes = userDocument.ArgumentVotes
            };
        }

        public static BasicUserCredentials ToDto(this UserDocument userDoc)
        {
            return new BasicUserCredentials
            {
                Id = userDoc.Id!,
                Username = userDoc.Username,
                CreatedAtUtc = userDoc.CreatedAtUtc,
            };
        }
    }
}

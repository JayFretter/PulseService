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
                PulseVotes = user.Votes
            };
        }

        public static UserDto ToDto(this UserDocument userDoc)
        {
            return new UserDto
            {
                Id = userDoc.Id!,
                Username = userDoc.Username,
                CreatedAtUtc = userDoc.CreatedAtUtc,
                Votes = userDoc.PulseVotes
            };
        }
    }
}

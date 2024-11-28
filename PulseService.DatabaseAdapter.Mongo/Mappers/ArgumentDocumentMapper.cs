using MongoDB.Bson;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Mappers;

public static class ArgumentDocumentMapper
{
    public static ArgumentDocument ToDocument(this DiscussionArgument discussionArgument)
    {
        return new ArgumentDocument
        {
            Id = discussionArgument.Id ?? ObjectId.GenerateNewId().ToString(),
            PulseId = discussionArgument.PulseId,
            ParentArgumentId = discussionArgument.ParentArgumentId,
            UserId = discussionArgument.UserId,
            Username = discussionArgument.Username,
            OpinionName = discussionArgument.OpinionName,
            ArgumentBody = discussionArgument.ArgumentBody,
            Upvotes = discussionArgument.Upvotes,
            Downvotes = discussionArgument.Downvotes,
        };
    }

    public static DiscussionArgument ToDomain(this ArgumentDocument argumentDocument)
    {
        return new DiscussionArgument
        {
            Id = argumentDocument.Id,
            ParentArgumentId = argumentDocument.ParentArgumentId,
            UserId = argumentDocument.UserId,
            Username = argumentDocument.Username,
            OpinionName = argumentDocument.OpinionName,
            ArgumentBody = argumentDocument.ArgumentBody,
            PulseId = argumentDocument.PulseId,
            Upvotes = argumentDocument.Upvotes,
            Downvotes = argumentDocument.Downvotes,
        };
    }
}
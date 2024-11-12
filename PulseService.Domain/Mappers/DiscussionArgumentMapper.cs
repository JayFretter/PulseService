using PulseService.Domain.Models;

namespace PulseService.Domain.Mappers
{
    internal static class DiscussionArgumentMapper
    {
        public static CollatedDiscussionArgument ToCollatedArgument(this DiscussionArgument argument)
        {
            return new CollatedDiscussionArgument
            {
                Id = argument.Id,
                UserId = argument.UserId,
                Username = argument.Username,
                OpinionName = argument.OpinionName,
                ArgumentBody = argument.ArgumentBody,
                PulseId = argument.PulseId,
                Upvotes = argument.Upvotes,
                Downvotes = argument.Downvotes,
                Children = Array.Empty<CollatedDiscussionArgument>()
            };
        }
    }
}

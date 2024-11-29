using PulseService.Domain.Models;

namespace PulseService.Api.Models.Responses;

public class GetChildArgumentsResponse
{
    public IEnumerable<CollatedDiscussionArgument> ChildArguments { get; set; } = Array.Empty<CollatedDiscussionArgument>();
}
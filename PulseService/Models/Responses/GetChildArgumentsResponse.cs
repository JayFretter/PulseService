using PulseService.Domain.Models;

namespace PulseService.Models.Responses;

public class GetChildArgumentsResponse
{
    public IEnumerable<CollatedDiscussionArgument> ChildArguments { get; set; } = Array.Empty<CollatedDiscussionArgument>();
}
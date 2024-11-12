namespace PulseService.Domain.Models
{
    public class CollatedDiscussionArgument
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string OpinionName { get; set; } = string.Empty;
        public string ArgumentBody { get; set; } = string.Empty;
        public string PulseId { get; set; } = string.Empty;
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public IEnumerable<CollatedDiscussionArgument> Children { get; set; } = Array.Empty<CollatedDiscussionArgument>();
    }
}

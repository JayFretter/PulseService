namespace PulseService.Domain.Models
{
    public class DiscussionArgument
    {
        public string Id { get; set; } = string.Empty;
        public string? ParentArgumentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string OpinionName { get; set; } = string.Empty;
        public string ArgumentBody { get; set; } = string.Empty;
        public string PulseId { get; set; } = string.Empty;
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
    }
}

namespace PulseService.Domain.Models
{
    public class DiscussionComment
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string OpinionName { get; set; } = string.Empty;
        public string OpinonBody { get; set; } = string.Empty;
        public string PulseId { get; set; } = string.Empty;
        public uint Upvotes { get; set; }
        public uint Downvotes { get; set; }
        public IEnumerable<DiscussionComment> Children { get; set; } = new List<DiscussionComment>();
    }
}

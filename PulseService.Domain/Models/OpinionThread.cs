namespace PulseService.Domain.Models
{
    public class OpinionThread
    {
        public string OpinionName { get; set; } = string.Empty;
        public IEnumerable<CollatedDiscussionComment> DiscussionComments = Array.Empty<CollatedDiscussionComment>();
    }
}

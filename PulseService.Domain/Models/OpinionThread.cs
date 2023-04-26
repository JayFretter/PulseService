namespace PulseService.Domain.Models
{
    public class OpinionThread
    {
        public string ThreadOpinionName { get; set; } = string.Empty;
        public IEnumerable<DiscussionComment> DiscussionComments = new List<DiscussionComment>();
    }
}

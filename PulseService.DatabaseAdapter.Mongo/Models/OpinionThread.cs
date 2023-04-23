using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Models
{
    public class OpinionThread
    {
        public string ThreadOpinionName { get; set; } = string.Empty;
        public IEnumerable<DiscussionComment> DiscussionComments = new List<DiscussionComment>();
    }
}

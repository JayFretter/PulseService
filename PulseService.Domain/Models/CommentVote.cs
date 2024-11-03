using PulseService.Domain.Enums;

namespace PulseService.Domain.Models
{
    public class CommentVote
    {
        public string CommentId { get; set; } = string.Empty;
        public CommentVoteStatus VoteStatus { get; set; }
    }
}

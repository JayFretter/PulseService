using PulseService.Domain.Enums;

namespace PulseService.Domain.Models
{
    public class CommentVoteUpdate
    {
        public string CommentId { get; set; } = string.Empty;
        public CommentVoteStatus VoteType { get; set; }
    }
}

using PulseService.Domain.Enums;

namespace PulseService.Domain.Models
{
    public class CommentVoteUpdate
    {
        public string CommentId { get; set; } = string.Empty;
        public CommentVoteType VoteType { get; set; }
        public string CurrentUserId { get; set; } = string.Empty;
    }
}

namespace PulseService.Models.Queries
{
    public class UpdateCommentVoteQuery
    {
        public string PulseId { get; set; } = string.Empty;
        public string CommentId { get; set; } = string.Empty;
        public int VoteType { get; set; }
    }
}

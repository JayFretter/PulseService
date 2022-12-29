namespace BiscuitService.Domain.Models
{
    public class VoteUpdate
    {
        public string BiscuitId { get; set; } = string.Empty;
        public string VotedOpinion { get; set; } = string.Empty;
        public string? UnvotedOpinion { get; set; }
        public string CurrentUserId { get; set; } = string.Empty;
    }
}

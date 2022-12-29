namespace PulseService.Domain.Models
{
    public class VoteUpdate
    {
        public string PulseId { get; set; } = string.Empty;
        public string? VotedOpinion { get; set; }
        public string? UnvotedOpinion { get; set; }
        public string CurrentUserId { get; set; } = string.Empty;
    }
}

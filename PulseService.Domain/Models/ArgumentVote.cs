using PulseService.Domain.Enums;

namespace PulseService.Domain.Models
{
    public class ArgumentVote
    {
        public string ArgumentId { get; set; } = string.Empty;
        public ArgumentVoteStatus VoteStatus { get; set; }
    }
}

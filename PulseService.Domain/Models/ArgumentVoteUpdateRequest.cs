using PulseService.Domain.Enums;

namespace PulseService.Domain.Models;

public class ArgumentVoteUpdateRequest
{
    public string ArgumentId { get; set; } = string.Empty;
    public ArgumentVoteStatus VoteType { get; set; }
}
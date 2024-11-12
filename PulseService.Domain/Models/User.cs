namespace PulseService.Domain.Models
{
    public class User
    {
        public string? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public PulseVote[] PulseVotes { get; set; } = Array.Empty<PulseVote>();
        public ArgumentVote[] ArgumentVotes { get; set; } = Array.Empty<ArgumentVote>();
    }
}

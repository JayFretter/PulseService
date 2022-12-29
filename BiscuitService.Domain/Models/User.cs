namespace BiscuitService.Domain.Models
{
    public class User
    {
        public string? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public Vote[] Votes { get; set; } = Array.Empty<Vote>();
    }
}

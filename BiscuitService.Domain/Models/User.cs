namespace BiscuitService.Domain.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public IEnumerable<Vote> Votes { get; set; } = new List<Vote>();
    }
}

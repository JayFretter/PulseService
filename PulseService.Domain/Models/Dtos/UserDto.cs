namespace PulseService.Domain.Models.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public Vote[] Votes { get; set; } = Array.Empty<Vote>();
    }
}

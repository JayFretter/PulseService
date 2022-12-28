namespace BiscuitService.Domain.Models.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public IEnumerable<Vote> Votes { get; set; } = new List<Vote>();
    }
}

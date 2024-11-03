namespace PulseService.Domain.Models.Dtos
{
    public class BasicUserCredentials
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}

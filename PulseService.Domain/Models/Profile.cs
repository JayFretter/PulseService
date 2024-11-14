namespace PulseService.Domain.Models
{
    public class Profile
    {
        public string Username { get; set; } = string.Empty;
        public IEnumerable<Pulse> Pulses { get; set; } = Array.Empty<Pulse>();
    }
}
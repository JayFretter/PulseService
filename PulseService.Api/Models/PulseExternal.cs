using PulseService.Domain.Models;

namespace PulseService.Api.Models;

public class PulseExternal
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
    public PulseUserDetails CreatedBy { get; set; } = new PulseUserDetails();
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
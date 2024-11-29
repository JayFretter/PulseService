using PulseService.Domain.Models;

namespace PulseService.Api.Models.Queries;

public class CreatePulseQuery
{
    public string Title { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
}
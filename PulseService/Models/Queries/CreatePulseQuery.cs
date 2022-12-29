using PulseService.Domain.Models;

namespace PulseService.Models.Queries
{
    public class CreatePulseQuery
    {
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
    }
}

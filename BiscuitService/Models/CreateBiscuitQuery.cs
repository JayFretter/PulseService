using BiscuitService.Domain.Models;

namespace BiscuitService.Models
{
    public class CreateBiscuitQuery
    {
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
    }
}

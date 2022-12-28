using BiscuitService.Domain.Models;

namespace BiscuitService.Models
{
    public class BiscuitExternal
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
    }
}

using BiscuitService.Domain.Models;

namespace BiscuitService.Models.Queries
{
    public class CreateBiscuitQuery
    {
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
    }
}

namespace BiscuitService.Domain.Models
{
    public class Biscuit
    {
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Opinion> Opinions { get; set; } = new List<Opinion>();
    }
}

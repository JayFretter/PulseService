using BiscuitService.Domain.Models;

namespace BiscuitService.Models
{
    public class BiscuitExternal
    {
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<OpinionExternal> Opinions { get; set; } = new List<OpinionExternal>();

        public static Biscuit ToDomain(BiscuitExternal biscuitExternal)
        {
            return new Biscuit
            {
                Id = biscuitExternal.Id,
                Title = biscuitExternal.Title,
                Opinions = biscuitExternal.Opinions.Select(
                    o => new Opinion
                    {
                        Name = o.Name,
                        Votes = o.Votes
                    })
            };
        }
    }
}

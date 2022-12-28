namespace BiscuitService.Models.Queries
{
    public class UpdateVoteQuery
    {
        public string BiscuitId { get; set; } = string.Empty;
        public string OptionName { get; set; } = string.Empty;
    }
}

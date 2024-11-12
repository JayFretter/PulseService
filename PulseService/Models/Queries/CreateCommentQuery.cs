namespace PulseService.Models.Queries
{
    public class CreateArgumentQuery
    {
        public string PulseId { get; set; } = string.Empty;
        public string? ParentArgumentId { get; set; }
        public string OpinionName { get; set; } = string.Empty;
        public string OpinionBody { get; set; } = string.Empty;
    }
}

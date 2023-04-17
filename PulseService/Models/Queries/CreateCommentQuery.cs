namespace PulseService.Models.Queries
{
    public class CreateCommentQuery
    {
        public string PulseId { get; set; } = string.Empty;
        public string? ParentCommentId { get; set; }
        public string OpinionName { get; set; } = string.Empty;
        public string OpinionBody { get; set; } = string.Empty;
    }
}

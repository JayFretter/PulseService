namespace PulseService.Api.Models.Queries;

public class UpdateVoteQuery
{
    public string PulseId { get; set; } = string.Empty;
    public string OptionName { get; set; } = string.Empty;
}
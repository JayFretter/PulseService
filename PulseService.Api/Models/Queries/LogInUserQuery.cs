namespace PulseService.Api.Models.Queries;

public class LogInUserQuery
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
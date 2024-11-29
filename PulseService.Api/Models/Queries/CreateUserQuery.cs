namespace PulseService.Api.Models.Queries;

public class CreateUserQuery
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
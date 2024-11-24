namespace PulseService.Models.Responses;

public class LoginSuccessfulResponse
{
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
namespace PulseService.Api.Models.Responses;

public class ValidationErrorResponse
{
    public string[] ValidationErrors { get; set; } = Array.Empty<string>();
}
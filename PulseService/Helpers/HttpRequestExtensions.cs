namespace PulseService.Helpers
{
    public static class HttpRequestExtensions
    {
        public static string GetBearerToken(this HttpRequest request)
        {
            return request.Headers.Authorization.ToString().Split("Bearer ")[1];
        }
    }
}

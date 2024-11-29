using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PulseService.Helpers;

namespace PulseService.Api.Tests.Unit.Helpers;

[TestFixture]
public class HttpRequestExtensionsTests
{
    [Test]
    public void GetBearerToken_WithValidAuthorizationHeader_ReturnsToken()
    {
        var token = "sample-token";
        var httpRequest = new DefaultHttpContext().Request;
        httpRequest.Headers.Authorization = $"Bearer {token}";

        var result = httpRequest.GetBearerToken();

        result.Should().Be(token);
    }

    [Test]
    public void GetBearerToken_WithNoBearerPrefix_ThrowsIndexOutOfRangeException()
    {
        var httpRequest = new DefaultHttpContext().Request;
        httpRequest.Headers.Authorization = "InvalidAuthHeader";

        Action act = () => httpRequest.GetBearerToken();

        act.Should().Throw<IndexOutOfRangeException>();
    }
}
using FluentAssertions;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using PulseService.Mappers;
using PulseService.Models.Queries;

namespace PulseService.Api.Tests.Unit.Mappers;

[TestFixture]
public class LogInUserQueryMapperTests
{
    [Test]
    public void ToDomain_CreatesUserCredentials_UsingLoginDetails()
    {
        var logInUserQuery = new LogInUserQuery
        {
            Username = "username",
            Password = "password123"
        };
        
        var result = logInUserQuery.ToDomain();

        var expectedResult = new UserCredentials
        {
            Username = "username",
            Password = "password123"
        };
        
        result.Should().BeEquivalentTo(expectedResult);
    }
}
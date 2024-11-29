using FluentAssertions;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using PulseService.Mappers;
using PulseService.Models.Queries;

namespace PulseService.Api.Tests.Unit.Mappers;

[TestFixture]
public class CreateUserQueryMapperTests
{
    [Test]
    public void ToDomain_CreatesUserObject_WithUserQueryDetails()
    {
        var query = new CreateUserQuery
        {
            Username = "user",
            Password = "password123"
        };

        var result = query.ToDomain();

        var expectedResult = new User
        {
            Username = "user",
            Password = "password123"
        };
        
        result.Should().BeEquivalentTo(expectedResult);
    }
}
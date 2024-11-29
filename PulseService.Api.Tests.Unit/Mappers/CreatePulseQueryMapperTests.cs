using FluentAssertions;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using PulseService.Api.Mappers;
using PulseService.Api.Models.Queries;

namespace PulseService.Api.Tests.Unit.Mappers;

[TestFixture]
public class CreatePulseQueryMapperTests
{
    [Test]
    public void ToDomain_CreatesPulse_WithPulseQueryAndUserDetails()
    {
        var query = new CreatePulseQuery
        {
            Title = "Pulse",
            Tags = "Fitness,Wellbeing,Finances",
            Opinions =
            [
                new Opinion {Name = "Opinion 1", Votes = 12},
                new Opinion {Name = "Opinion 2", Votes = 52},
            ]
        };

        var userCredentials = new BasicUserCredentials
        {
            Username = "test_username",
            Id = "userId",
            CreatedAtUtc = new DateTime(2024, 10, 15, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = query.ToDomain(userCredentials);

        var expectedResult = new Pulse
        {
            Title = "Pulse",
            Tags = "Fitness,Wellbeing,Finances",
            Opinions = 
            [
                new Opinion {Name = "Opinion 1", Votes = 12},
                new Opinion {Name = "Opinion 2", Votes = 52},
            ],
            CreatedBy = new PulseUserDetails
            {
                Id = "userId",
                Username = "test_username"
            },
        };
        
        result.Should().BeEquivalentTo(expectedResult);
    }
}
using FluentAssertions;
using PulseService.Domain.Models;
using PulseService.Mappers;
using PulseService.Models;

namespace PulseService.Api.Tests.Unit.Mappers;

[TestFixture]
public class PulseExternalMapperTests
{
    [Test]
    public void ToExternal_CreatesPulseExternalObject_WithCorrectlyMappedValues()
    {
        var domainPulse = new Pulse
        {
            Id = "pulseId",
            Title = "test title",
            Tags = "tag1,tag2",
            Opinions =
            [
                new Opinion {Name = "Opinion 1", Votes = 12},
                new Opinion {Name = "Opinion 2", Votes = 52},
            ],
            CreatedBy = new PulseUserDetails
            {
                Id = "userId",
                Username = "username",
            },
            CreatedAtUtc = new DateTime(2024, 1, 1, 19, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2024, 1, 23, 11, 30, 0, DateTimeKind.Utc)
        };
        
        var result = domainPulse.ToExternal();

        var expectedResult = new PulseExternal
        {
            Id = "pulseId",
            Title = "test title",
            Tags = "tag1,tag2",
            Opinions =
            [
                new Opinion {Name = "Opinion 1", Votes = 12},
                new Opinion {Name = "Opinion 2", Votes = 52},
            ],
            CreatedBy = new PulseUserDetails
            {
                Id = "userId",
                Username = "username",
            },
            CreatedAtUtc = new DateTime(2024, 1, 1, 19, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2024, 1, 23, 11, 30, 0, DateTimeKind.Utc)
        };
        
        result.Should().BeEquivalentTo(expectedResult);
    }
}
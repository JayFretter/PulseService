using FluentAssertions;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using PulseService.Api.Mappers;
using PulseService.Api.Models.Queries;

namespace PulseService.Api.Tests.Unit.Mappers;

[TestFixture]
public class CreateArgumentQueryMapperTests
{
    [Test]
    public void ToDomain_CreatesDiscussionArgument_WithArgumentQueryAndUserDetails()
    {
        var query = new CreateArgumentQuery
        {
            ParentArgumentId = "parentId",
            OpinionName = "opinionName",
            PulseId = "pulseId",
            OpinionBody = "opinionBody"
        };

        var userCredentials = new BasicUserCredentials
        {
            Username = "test_username",
            Id = "userId",
            CreatedAtUtc = new DateTime(2024, 10, 15, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = query.ToDomain(userCredentials);

        var expectedResult = new DiscussionArgument
        {
            ParentArgumentId = "parentId",
            UserId = "userId",
            Username = "test_username",
            OpinionName = "opinionName",
            PulseId = "pulseId",
            ArgumentBody = "opinionBody",
            Upvotes = 0,
            Downvotes = 0
        };
        
        result.Should().BeEquivalentTo(expectedResult);
    }
}
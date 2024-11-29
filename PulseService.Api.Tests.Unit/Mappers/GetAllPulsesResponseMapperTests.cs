using FluentAssertions;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using PulseService.Api.Mappers;
using PulseService.Api.Models.Queries;

namespace PulseService.Api.Tests.Unit.Mappers;

[TestFixture]
public class GetAllPulsesResponseMapperTests
{
    [Test]
    public void ToExternal_MapsOnlyPulsesWithIds()
    {
        Pulse[] pulses =
        [
            new Pulse
            {
                Id = "1",
                Title = "Pulse with ID 1"
            },
            new Pulse
            {
                Id = "2",
                Title = "Pulse with ID 2"
            },
            new Pulse
            {
                Id = null,
                Title = "Pulse without Id"
            }
        ];
        
        var result = pulses.ToExternal();
        
        result.Should().BeEquivalentTo([pulses[0], pulses[1]]);
    }
}
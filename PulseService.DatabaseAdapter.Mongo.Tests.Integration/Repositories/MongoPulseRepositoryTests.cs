using FluentAssertions;
using MongoDB.Driver;
using NUnit.Framework;
using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.DatabaseAdapter.Mongo.Repositories;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Tests.Integration.Repositories;

[TestFixture]
public class MongoPulseRepositoryTests : TestBase
{
    private const string MockDocumentId = "000011112222333344445555";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    
    private IMongoCollection<PulseDocument> _collection;
    private MongoPulseRepository _repository;

    [OneTimeSetUp]
    public new void OneTimeSetUp()
    {
        _collection = MongoService.GetDatabase().GetCollection<PulseDocument>(MongoOptions.Value.PulseCollectionName);
        _repository = new MongoPulseRepository(MongoService, MongoOptions);
    }

    [SetUp]
    public async Task Setup()
    {
        await _collection.DeleteManyAsync(FilterDefinition<PulseDocument>.Empty, cancellationToken: _cancellationToken);
    }

    [Test]
    public async Task AddPulseAsync_AddsPulse()
    {
        var pulse = new Pulse
        {
            Id = MockDocumentId,
            Title = "Test Pulse",
            CreatedBy = new PulseUserDetails { Id = "user1" },
            Opinions = new List<Opinion>
            {
                new() { Name = "Yes", Votes = 10 },
                new() { Name = "No", Votes = 5 }
            }
        };

        await _repository.AddPulseAsync(pulse, _cancellationToken);

        var result = await _collection.Find(x => x.Id == pulse.Id).FirstOrDefaultAsync(_cancellationToken);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(pulse.ToDocument());
    }

    [Test]
    public async Task DeletePulseAsync_WithValidIdAndUser_DeletesPulse()
    {
        var pulse = new Pulse
        {
            Id = MockDocumentId,
            Title = "Test Pulse",
            CreatedBy = new PulseUserDetails { Id = "user1" },
            Opinions = new List<Opinion>()
        };

        await _collection.InsertOneAsync(pulse.ToDocument(), cancellationToken: _cancellationToken);

        var result = await _repository.DeletePulseAsync(pulse.Id, "user1", _cancellationToken);

        result.Should().BeTrue();

        var deletedPulse = await _collection.Find(x => x.Id == pulse.Id).FirstOrDefaultAsync(_cancellationToken);
        deletedPulse.Should().BeNull();
    }

    [Test]
    public async Task DeletePulseAsync_WithInvalidUserId_ReturnsFalse()
    {
        var pulse = new Pulse
        {
            Id = MockDocumentId,
            Title = "Test Pulse",
            CreatedBy = new PulseUserDetails { Id = "user1" },
            Opinions = new List<Opinion>()
        };

        await _collection.InsertOneAsync(pulse.ToDocument(), cancellationToken: _cancellationToken);

        var result = await _repository.DeletePulseAsync(pulse.Id, "wrongUserId", _cancellationToken);

        result.Should().BeFalse();

        var existingPulse = await _collection.Find(x => x.Id == pulse.Id).FirstOrDefaultAsync(_cancellationToken);
        existingPulse.Should().NotBeNull();
    }

    [Test]
    public async Task GetAllPulsesAsync_ReturnsAllPulses()
    {
        var pulses = new[]
        {
            new Pulse
            {
                Title = "Pulse 1",
                CreatedBy = new PulseUserDetails { Id = "user1" }
            },
            new Pulse
            {
                Title = "Pulse 2",
                CreatedBy = new PulseUserDetails { Id = "user2" }
            }
        };

        await _collection.InsertManyAsync(pulses.Select(p => p.ToDocument()), cancellationToken: _cancellationToken);

        var result = await _repository.GetAllPulsesAsync(_cancellationToken);

        result.Should().BeEquivalentTo(pulses, options => options.Excluding(p => p.Id));
    }

    [Test]
    public async Task GetPulseAsync_WithValidId_ReturnsPulse()
    {
        var pulse = new Pulse
        {
            Id = MockDocumentId,
            Title = "Test Pulse",
            CreatedBy = new PulseUserDetails { Id = "user1" },
            Opinions = new List<Opinion>()
        };

        await _collection.InsertOneAsync(pulse.ToDocument(), cancellationToken: _cancellationToken);

        var result = await _repository.GetPulseAsync(pulse.Id, _cancellationToken);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(pulse);
    }

    [Test]
    public async Task GetPulseAsync_WithInvalidId_ReturnsNull()
    {
        var nonExistentId = "aaaaaabbbbbbccccccdddddd";
        var result = await _repository.GetPulseAsync(nonExistentId, _cancellationToken);

        result.Should().BeNull();
    }

    [Test]
    public async Task UpdatePulseVoteAsync_UpdatesVoteCounts()
    {
        var pulse = new Pulse
        {
            Id = MockDocumentId,
            Title = "Test Pulse",
            CreatedBy = new PulseUserDetails { Id = "user1" },
            Opinions = new List<Opinion>
            {
                new() { Name = "Yes", Votes = 10 },
                new() { Name = "No", Votes = 5 }
            }
        };

        await _collection.InsertOneAsync(pulse.ToDocument(), cancellationToken: _cancellationToken);

        var voteUpdate = new VoteUpdate
        {
            PulseId = pulse.Id,
            VotedOpinion = "Yes",
            UnvotedOpinion = "No"
        };

        await _repository.UpdatePulseVoteAsync(voteUpdate, _cancellationToken);

        var updatedPulse = await _collection.Find(x => x.Id == pulse.Id).FirstOrDefaultAsync(_cancellationToken);

        updatedPulse.Opinions.First(o => o.Name == "Yes").Votes.Should().Be(11);
        updatedPulse.Opinions.First(o => o.Name == "No").Votes.Should().Be(4);
    }

    [Test]
    public async Task GetPulsesByUserIdAsync_ReturnsPulsesForUser()
    {
        var pulses = new[]
        {
            new Pulse { Title = "Pulse 1", CreatedBy = new PulseUserDetails { Id = "user1" } },
            new Pulse { Title = "Pulse 2", CreatedBy = new PulseUserDetails { Id = "user1" } },
            new Pulse { Title = "Pulse 3", CreatedBy = new PulseUserDetails { Id = "user2" } }
        };

        await _collection.InsertManyAsync(pulses.Select(p => p.ToDocument()), cancellationToken: _cancellationToken);

        var result = await _repository.GetPulsesByUserIdAsync("user1", _cancellationToken);

        result.Should().HaveCount(2).And.OnlyContain(p => p.CreatedBy.Id == "user1");
    }
}
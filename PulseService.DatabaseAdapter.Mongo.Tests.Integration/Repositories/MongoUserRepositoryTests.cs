using FluentAssertions;
using MongoDB.Driver;
using NUnit.Framework;
using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.DatabaseAdapter.Mongo.Repositories;
using PulseService.Domain.Enums;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Tests.Integration.Repositories;

[TestFixture]
public class MongoUserRepositoryTests : TestBase
{
    private const string MockDocumentId = "000011112222333344445555";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    
    private IMongoCollection<UserDocument> _collection;
    private MongoUserRepository _repository;

    [OneTimeSetUp]
    public new void OneTimeSetUp()
    {
        _collection = MongoService.GetDatabase().GetCollection<UserDocument>(MongoOptions.Value.UserCollectionName);
        _repository = new MongoUserRepository(MongoService, MongoOptions);
    }

    [SetUp]
    public async Task Setup()
    {
        await _collection.DeleteManyAsync(FilterDefinition<UserDocument>.Empty, cancellationToken: _cancellationToken);
    }

    [Test]
    public async Task AddUserAsync_AddsUser()
    {
        var user = new User
        {
            Id = MockDocumentId,
            Username = "TestUser",
            Password = "Password123",
        };

        await _repository.AddUserAsync(user, _cancellationToken);

        var result = await _collection.Find(u => u.Id == user.Id).FirstOrDefaultAsync(_cancellationToken);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user.ToDocument());
    }

    [Test]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        var user = new User
        {
            Id = MockDocumentId,
            Username = "TestUser",
            Password = "Password123"
        };

        await _collection.InsertOneAsync(user.ToDocument(), cancellationToken: _cancellationToken);

        var result = await _repository.GetUserByIdAsync(user.Id, _cancellationToken);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user);
    }

    [Test]
    public async Task GetUserByIdAsync_UserIdDoesntExist_ReturnsNull()
    {
        var nonExistentId = "aaaabbbbccccddddeeeeffff";
        
        var result = await _repository.GetUserByIdAsync(nonExistentId, _cancellationToken);

        result.Should().BeNull();
    }

    [Test]
    public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUserAsBasicUserCredentials()
    {
        var userDocument = new UserDocument
        {
            Id = MockDocumentId,
            Username = "TestUser",
            Password = "Password123"
        };

        await _collection.InsertOneAsync(userDocument, cancellationToken: _cancellationToken);

        var result = await _repository.GetUserByUsernameAsync(userDocument.Username, _cancellationToken);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDocument.ToDto());
    }

    [Test]
    public async Task GetUserByUsernameAsync_UsernameDoesntExist_ReturnsNull()
    {
        var result = await _repository.GetUserByUsernameAsync("NonExistentUser", _cancellationToken);
        result.Should().BeNull();
    }

    [Test]
    public async Task UpdatePulseVoteAsync_UpdatesExistingVote()
    {
        var user = new User
        {
            Id = MockDocumentId,
            Username = "TestUser",
            PulseVotes =
            [
                new PulseVote { PulseId = "Pulse1", OpinionName = "Yes" }
            ]
        };

        await _collection.InsertOneAsync(user.ToDocument(), cancellationToken: _cancellationToken);

        var voteUpdate = new VoteUpdate
        {
            CurrentUserId = user.Id,
            PulseId = "Pulse1",
            VotedOpinion = "No"
        };

        await _repository.UpdatePulseVoteAsync(voteUpdate, _cancellationToken);

        var updatedUser = await _collection.Find(u => u.Id == user.Id).FirstOrDefaultAsync(_cancellationToken);

        updatedUser.PulseVotes.Should().ContainSingle(v => v.PulseId == "Pulse1" && v.OpinionName == "No");
    }

    [Test]
    public async Task UpdatePulseVoteAsync_AddsNewVoteWhenNoneExists()
    {
        var user = new User
        {
            Id = MockDocumentId,
            Username = "TestUser"
        };

        await _collection.InsertOneAsync(user.ToDocument(), cancellationToken: _cancellationToken);

        var voteUpdate = new VoteUpdate
        {
            CurrentUserId = user.Id,
            PulseId = "Pulse1",
            VotedOpinion = "Yes"
        };

        await _repository.UpdatePulseVoteAsync(voteUpdate, _cancellationToken);

        var updatedUser = await _collection.Find(u => u.Id == user.Id).FirstOrDefaultAsync(_cancellationToken);

        updatedUser.PulseVotes.Should().ContainSingle(v => v.PulseId == "Pulse1" && v.OpinionName == "Yes");
    }

    [Test]
    public async Task RemoveArgumentVoteStatusAsync_RemovesVote()
    {
        var user = new User
        {
            Id = MockDocumentId,
            ArgumentVotes =
            [
                new ArgumentVote { ArgumentId = "Arg1", VoteStatus = ArgumentVoteStatus.Upvote }
            ]
        };

        await _collection.InsertOneAsync(user.ToDocument(), cancellationToken: _cancellationToken);

        await _repository.RemoveArgumentVoteStatusAsync(user.Id, "Arg1", _cancellationToken);

        var updatedUser = await _collection.Find(u => u.Id == user.Id).FirstOrDefaultAsync(_cancellationToken);

        updatedUser.ArgumentVotes.Should().BeEmpty();
    }

    [Test]
    public async Task RemoveArgumentVoteStatusAsync_NoMatchingVote_DoesNothing()
    {
        var user = new User
        {
            Id = MockDocumentId,
            ArgumentVotes =
            [
                new ArgumentVote { ArgumentId = "Arg1", VoteStatus = ArgumentVoteStatus.Upvote }
            ]
        };

        await _collection.InsertOneAsync(user.ToDocument(), cancellationToken: _cancellationToken);

        await _repository.RemoveArgumentVoteStatusAsync(user.Id, "NonexistentArg", _cancellationToken);

        var updatedUser = await _collection.Find(u => u.Id == user.Id).FirstOrDefaultAsync(_cancellationToken);

        updatedUser.ArgumentVotes.Should().ContainSingle(v => v.ArgumentId == "Arg1");
    }
}
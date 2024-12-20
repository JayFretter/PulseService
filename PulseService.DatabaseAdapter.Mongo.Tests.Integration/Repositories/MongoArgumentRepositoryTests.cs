﻿using FluentAssertions;
using MongoDB.Driver;
using NUnit.Framework;
using PulseService.DatabaseAdapter.Mongo.Mappers;
using PulseService.DatabaseAdapter.Mongo.Models;
using PulseService.DatabaseAdapter.Mongo.Repositories;
using PulseService.Domain.Models;

namespace PulseService.DatabaseAdapter.Mongo.Tests.Integration.Repositories;

[TestFixture]
public class MongoArgumentRepositoryTests : TestBase
{
    private const int BaseResultLimit = 100;
    private const string MockDocumentId = "000011112222333344445555";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private IMongoCollection<ArgumentDocument> _collection;

    private MongoArgumentRepository _repository;

    [OneTimeSetUp]
    public new void OneTimeSetUp()
    {
        _collection = MongoService.GetDatabase()
            .GetCollection<ArgumentDocument>(MongoOptions.Value.ArgumentCollectionName);

        _repository = new MongoArgumentRepository(MongoService, MongoOptions);
    }
    
    [SetUp]
    public async Task Setup()
    {
        await _collection.DeleteManyAsync(FilterDefinition<ArgumentDocument>.Empty,
            cancellationToken: _cancellationToken);
    }

    [Test]
    public async Task AddArgumentAsync_AddsArgument()
    {
        var resultBeforeAdd = (await _collection.FindAsync(FilterDefinition<ArgumentDocument>.Empty,
                cancellationToken: _cancellationToken))
            .ToList();

        var argument = new DiscussionArgument
        {
            Id = MockDocumentId,
            PulseId = "92817391274",
            OpinionName = "Yes",
            ArgumentBody = "Test argument",
            ParentArgumentId = "198641279612",
            Upvotes = 1,
            Downvotes = 1,
            Username = "user1",
            UserId = "178228764",
        };

        await _repository.AddArgumentAsync(argument, _cancellationToken);

        var resultAfterAdd = (await _collection.FindAsync(a => true, cancellationToken: _cancellationToken))
            .ToList();

        resultBeforeAdd.Should().BeEmpty();
        resultAfterAdd.Should().HaveCount(1).And.BeEquivalentTo([argument.ToDocument()]);
    }

    [Test]
    public async Task GetTopLevelArgumentsForPulseIdAsync_ArgumentsHaveGivenPulseId_ReturnsTopLevelArguments()
    {
        var targetPulseId = "12345";

        var arguments = new[]
        {
            new DiscussionArgument
            {
                PulseId = targetPulseId,
                OpinionName = "Yes",
                ArgumentBody = "Test argument",
                ParentArgumentId = null,
                Upvotes = 1,
                Downvotes = 1,
                Username = "user1",
                UserId = "178228764",
            },

            new DiscussionArgument
            {
                PulseId = targetPulseId,
                OpinionName = "No",
                ArgumentBody = "Test argument 2",
                ParentArgumentId = null,
                Upvotes = 1,
                Downvotes = 1,
                Username = "user1",
                UserId = "178228764",
            },

            new DiscussionArgument
            {
                PulseId = "differentPulseId", // Should ignore as it has a different Pulse ID
                OpinionName = "Yes",
                ArgumentBody = "Test argument 3",
                ParentArgumentId = null,
                Upvotes = 1,
                Downvotes = 1,
                Username = "user1",
                UserId = "178228764",
            }
        };

        await _collection.InsertManyAsync(arguments.Select(x => x.ToDocument()), cancellationToken: _cancellationToken);

        var result =
            await _repository.GetTopLevelArgumentsForPulseIdAsync(targetPulseId, BaseResultLimit,
                cancellationToken: _cancellationToken);

        result.Should().BeEquivalentTo([arguments[0], arguments[1]], options => options.Excluding(a => a.Id));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async Task GetTopLevelArgumentsForPulseIdAsync_GivenLimit_LimitsResultCountToGivenLimit(int limit)
    {
        var targetPulseId = "12345";

        var arguments = Enumerable.Range(1, 3).Select(i => new DiscussionArgument
        {
            PulseId = targetPulseId,
            ParentArgumentId = null,
            ArgumentBody = $"Child argument {i}",
        });

        await _collection.InsertManyAsync(arguments.Select(x => x.ToDocument()), cancellationToken: _cancellationToken);

        var result =
            await _repository.GetTopLevelArgumentsForPulseIdAsync(targetPulseId, limit,
                cancellationToken: _cancellationToken);

        result.Should().HaveCount(limit);
    }

    [Test]
    public async Task GetTopLevelArgumentsForPulseIdAsync_GivenNoDocuments_ReturnsEmptyCollection()
    {
        var targetPulseId = "12345";

        var result =
            await _repository.GetTopLevelArgumentsForPulseIdAsync(targetPulseId, BaseResultLimit,
                cancellationToken: _cancellationToken);

        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetChildrenOfArgumentIdAsync_GivenParentArgumentId_ReturnsChildren()
    {
        var parentArgumentId = "parent123";

        var childArguments = new[]
        {
            new DiscussionArgument
            {
                ParentArgumentId = parentArgumentId,
                PulseId = "pulse1",
                ArgumentBody = "Child argument 1",
                Username = "user1",
                UserId = "user1_id"
            },
            new DiscussionArgument
            {
                ParentArgumentId = parentArgumentId,
                PulseId = "pulse1",
                ArgumentBody = "Child argument 2",
                Username = "user2",
                UserId = "user2_id"
            },
            new DiscussionArgument
            {
                ParentArgumentId = "differentParentId", // Should ignore as the parent ID is different
                PulseId = "pulse1",
                ArgumentBody = "Unrelated argument",
                Username = "user3",
                UserId = "user3_id"
            }
        };

        await _collection.InsertManyAsync(childArguments.Select(x => x.ToDocument()),
            cancellationToken: _cancellationToken);

        var result =
            await _repository.GetChildrenOfArgumentIdAsync(parentArgumentId, BaseResultLimit, _cancellationToken);

        result.Should().BeEquivalentTo([childArguments[0], childArguments[1]], options => options.Excluding(a => a.Id));
    }

    [TestCase(1)]
    [TestCase(2)]
    public async Task GetChildrenOfArgumentIdAsync_LimitsResultsToSpecifiedLimit(int limit)
    {
        var parentArgumentId = "parent123";

        var childArguments = Enumerable.Range(1, 3).Select(i => new DiscussionArgument
        {
            ParentArgumentId = parentArgumentId,
            PulseId = "pulse1",
            ArgumentBody = $"Child argument {i}",
            Username = $"user{i}",
            UserId = $"user{i}_id"
        });

        await _collection.InsertManyAsync(childArguments.Select(x => x.ToDocument()),
            cancellationToken: _cancellationToken);

        var result = await _repository.GetChildrenOfArgumentIdAsync(parentArgumentId, limit, _cancellationToken);

        result.Should().HaveCount(limit);
    }

    [Test]
    public async Task AdjustArgumentVotesAsync_UpdatesVoteCounts()
    {
        var argument = new DiscussionArgument
        {
            Id = MockDocumentId,
            PulseId = "pulse1",
            ArgumentBody = "Test argument",
            Upvotes = 5,
            Downvotes = 2,
            Username = "user1",
            UserId = "user1_id"
        };

        await _collection.InsertOneAsync(argument.ToDocument(), cancellationToken: _cancellationToken);

        await _repository.AdjustArgumentVotesAsync(argument.Id, 3, 1, _cancellationToken);

        var updatedArgument = await _collection.Find(a => a.Id == argument.Id).FirstOrDefaultAsync(_cancellationToken);

        updatedArgument.Upvotes.Should().Be(8);
        updatedArgument.Downvotes.Should().Be(3);
    }

    [Test]
    public async Task SetArgumentToDeletedAsync_MarksArgumentAsDeleted()
    {
        var argument = new DiscussionArgument
        {
            Id = MockDocumentId,
            PulseId = "pulse1",
            ArgumentBody = "Original body",
            Username = "originalUser",
            UserId = "user1_id"
        };

        await _collection.InsertOneAsync(argument.ToDocument(), cancellationToken: _cancellationToken);

        await _repository.SetArgumentToDeletedAsync(argument.UserId, argument.Id, _cancellationToken);

        var updatedArgument = await _collection.Find(a => a.Id == argument.Id).FirstOrDefaultAsync(_cancellationToken);

        updatedArgument.Username.Should().Be("[deleted]");
        updatedArgument.ArgumentBody.Should().Be("[deleted]");
    }

    [Test]
    public async Task SetArgumentToDeletedAsync_WrongUserId_DoesNotMarkAsDeleted()
    {
        var argument = new DiscussionArgument
        {
            Id = MockDocumentId,
            PulseId = "pulse1",
            ArgumentBody = "Original body",
            Username = "originalUser",
            UserId = "user1_id"
        };

        await _collection.InsertOneAsync(argument.ToDocument(), cancellationToken: _cancellationToken);

        await _repository.SetArgumentToDeletedAsync("wrong_user_id", argument.Id, _cancellationToken);

        var updatedArgument = await _collection.Find(a => a.Id == argument.Id).FirstOrDefaultAsync(_cancellationToken);

        updatedArgument.Username.Should().Be("originalUser");
        updatedArgument.ArgumentBody.Should().Be("Original body");
    }
}
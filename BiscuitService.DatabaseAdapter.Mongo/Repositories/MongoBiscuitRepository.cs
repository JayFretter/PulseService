﻿using BiscuitService.DatabaseAdapter.Mongo.Models;
using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BiscuitService.DatabaseAdapter.Mongo.Repositories
{
    public class MongoBiscuitRepository : IBiscuitRepository
    {
        private readonly IMongoCollection<BiscuitDbo> _collection;
        public MongoBiscuitRepository(MongoService service, IOptions<MongoOptions> mongoOptions)
        {
            _collection = service.GetDatabase().GetCollection<BiscuitDbo>(mongoOptions.Value.BiscuitCollectionName);
        }

        public async Task AddBiscuitAsync(Biscuit biscuit)
        {
            var biscuitDbo = BiscuitDbo.FromDomain(biscuit);

            await _collection.InsertOneAsync(biscuitDbo);
        }
    }
}

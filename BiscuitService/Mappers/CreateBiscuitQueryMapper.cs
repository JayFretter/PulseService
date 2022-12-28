﻿using BiscuitService.Domain.Models;
using BiscuitService.Models.Queries;

namespace BiscuitService.Mappers
{
    public static class CreateBiscuitQueryMapper
    {
        public static Biscuit ToDomain(this CreateBiscuitQuery query) 
        {
            return new Biscuit
            {
                Id = query.Id,
                Title = query.Title,
                Opinions = query.Opinions,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}

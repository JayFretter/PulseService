﻿using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;
using PulseService.Models.Queries;

namespace PulseService.Mappers
{
    public static class CreateCommentQueryMapper
    {
        public static DiscussionComment ToDomain(this CreateCommentQuery query, BasicUserCredentials currentUser) 
        {
            return new DiscussionComment
            {
                ParentCommentId = query.ParentCommentId,
                UserId = currentUser.Id,
                Username = currentUser.Username,
                OpinionName = query.OpinionName,
                CommentBody = query.OpinionBody,
                PulseId = query.PulseId,
                Upvotes = 0,
                Downvotes = 0
            };
        }
    }
}

using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using Imagegram.Api.Exceptions;
using Imagegram.Api.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class GetPostsHandler : IRequestHandler<GetPostsRequest, GetPostsResponse>
    {
        private const int MaxLimit = 1000;
        private readonly ApplicationContext _db;
        private Cash<AccountModel> _accountsCash;

        public GetPostsHandler(ApplicationContext db, Cash<AccountModel> accountsCash)
        {
            _db = db;
            _accountsCash = accountsCash;
        }

        public async Task<GetPostsResponse> Handle(GetPostsRequest request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            var account = await GetAccount(request.AccountId, cancellationToken);
            var cursor = PostsCursor.ParseCursor(request.Cursor);            
            var limit = request.Limit;

            var postsQuery = GetPostsQuery(cursor, limit);
            var posts = await postsQuery.ToListAsync(cancellationToken);
            var nextCursor = GetNextCursor(limit, posts);

            return new GetPostsResponse
            {
                Posts = DtosBuilder.Build(posts, account),
                Cursor = nextCursor.ToBase64()
            };
        }

        private void ValidateRequest(GetPostsRequest request)
        {
            var cursor = PostsCursor.ParseCursor(request.Cursor);
            if (cursor.IsInvalid)
            {
                throw new InvalidParameterException("Cursor has invalid format.");
            }

            var limit = request.Limit;
            if (limit > MaxLimit)
            {
                throw new InvalidParameterException("The limit parameter can't be greater than 1000.");
            }
        }

        private async Task<AccountModel> GetAccount(Guid accountId, CancellationToken cancellationToken)
        {
            return await _accountsCash.GetOrCreate(accountId,
                async () => await _db.Accounts.FindAsync(accountId, cancellationToken));
        }

        private static PostsCursor GetNextCursor(int limit, List<PostModel> posts)
        {
            if (posts.Count < limit)
            {
                return PostsCursor.EmptyCursor();
            }

            var lastPost = posts.Last();
            return PostsCursor.NewCursor(lastPost.CommentsCount, lastPost.Id);
        }

        private IQueryable<PostModel> GetPostsQuery(PostsCursor currentCursor, int limit)
        {
            IQueryable<PostModel> query = _db.Posts;

            if (currentCursor.IsEmpty)
            {
                query = query
                    .OrderByDescending(p => p.CommentsCount)
                    .ThenBy(p => p.Id);
            }
            else
            {
                query = query
                    .Where(p =>
                        (p.CommentsCount < currentCursor.CommentsCount) ||
                        (p.CommentsCount == currentCursor.CommentsCount && p.Id > currentCursor.LastPostId))
                    .OrderByDescending(p => p.CommentsCount)
                    .ThenBy(p => p.Id);
            }

            query = query 
                .Take(limit)
                .Include(p => p.CommentLast)
                .ThenInclude(c => c.Creator)
                .Include(p => p.CommentBeforeLast)
                .ThenInclude(c => c.Creator);

            return query;
        }
    }
}

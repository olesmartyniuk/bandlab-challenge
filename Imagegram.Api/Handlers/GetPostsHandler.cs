using AutoMapper;
using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using Imagegram.Api.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        public GetPostsHandler(ApplicationContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<GetPostsResponse> Handle(GetPostsRequest request, CancellationToken cancellationToken)
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

            var postsQuery = GetPostsQuery(cursor, limit);
            var posts = await postsQuery.ToListAsync(cancellationToken);
            var nextCursor = GetNextCursor(limit, posts);

            return new GetPostsResponse
            {
                Posts = _mapper.Map<List<PostDto>>(posts),
                Cursor = nextCursor.ToBase64()
            };
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
            if (currentCursor.IsEmpty)
            {
                return _db.Posts
                    .OrderByDescending(p => p.CommentsCount)
                    .ThenBy(p => p.Id)
                    .Take(limit)
                    .Include(p => p.CommentLast)
                    .Include(p => p.CommentBeforeLast);
            }

            return _db.Posts
                .Where(p =>
                    (p.CommentsCount < currentCursor.CommentsCount) ||
                    (p.CommentsCount == currentCursor.CommentsCount && p.Id > currentCursor.LastPostId))
                .OrderByDescending(p => p.CommentsCount)
                .ThenBy(p => p.Id)
                .Take(limit)
                .Include(p => p.CommentLast)
                .Include(p => p.CommentBeforeLast);
        }
    }
}

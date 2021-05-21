using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using Imagegram.Api.Exceptions;
using Imagegram.Api.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class AddCommentHandler : IRequestHandler<AddCommentRequest, CommentDto>
    {
        private readonly ApplicationContext _db;
        private readonly Cache<PostModel> _postsCache;
        private readonly DateTimeService _dateTime;
        private readonly Cache<AccountModel> _accountsCache;

        public AddCommentHandler(ApplicationContext db, Cache<PostModel> postsCache, DateTimeService dateTime, Cache<AccountModel> accountsCache)
        {
            _db = db;
            _postsCache = postsCache;
            _dateTime = dateTime;
            _accountsCache = accountsCache;
        }

        public async Task<CommentDto> Handle(AddCommentRequest request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            var account = await GetAccount(request.AccountId, cancellationToken);
            var post = await GetPost(request.PostId);

            var comment = new CommentModel
            {
                Content = request.Content,
                CreatedAt = _dateTime.Now(),
                PostId = post.Id,
                CreatorId = account.Id
            };

            // TODO: in one transaction
            await SaveComment(comment);
            await UpdatePost(post);

            return DtosBuilder.Build(comment, account);
        }

        private async Task<PostModel> GetPost(int postId)
        {
            var post = await _postsCache.GetOrCreate(postId,
                async () => await _db.Posts.FindAsync(postId));
            if (post == null)
            {
                throw new PostNotFoundException($"Post with id '{postId}' is not found.");
            }

            return post;
        }

        private async Task<AccountModel> GetAccount(Guid accountId, CancellationToken cancellationToken)
        {
            return await _accountsCache.GetOrCreate(accountId,
                async () => await _db.Accounts.FindAsync(accountId, cancellationToken));
        }

        private static void ValidateRequest(AddCommentRequest request)
        {
            if (string.IsNullOrEmpty(request.Content))
            {
                throw new InvalidParameterException("Comment content can't be empty.");
            }
        }

        private async Task SaveComment(CommentModel comment)
        {
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
        }

        private async Task UpdatePost(PostModel post)
        {
            var commandText = @"
                update Posts
                set 
                CommentsCount = (
	                select count(id) 
	                from Comments 
	                where PostId = {0}), 
                CommentLastId = (
	                select Id 
	                from Comments 
	                where PostId = {0} 
	                order by CreatedAt desc
	                limit 1),
                CommentBeforeLastId = (
	                select Id 
	                from Comments 
	                where PostId = {0}
	                order by CreatedAt desc
	                limit 1
                    offset 1)
                where Id = {0}";
            await _db.Database.ExecuteSqlRawAsync(commandText, post.Id);
        }
    }
}

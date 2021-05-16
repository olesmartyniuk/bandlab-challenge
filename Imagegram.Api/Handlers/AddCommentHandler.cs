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
        private readonly Cash<PostModel> _postsCash;
        private readonly DateTimeService _dateTime;
        private readonly Cash<AccountModel> _accountsCash;

        public AddCommentHandler(ApplicationContext db, Cash<PostModel> postsCash, DateTimeService dateTime, Cash<AccountModel> accountsCash)
        {
            _db = db;
            _postsCash = postsCash;
            _dateTime = dateTime;
            _accountsCash = accountsCash;
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

            await SaveComment(comment);
            await UpdatePost(post);

            return DtosBuilder.Build(comment, account);
        }

        private async Task<PostModel> GetPost(int postId)
        {
            var post = await _postsCash.GetOrCreate(postId,
                async () => await _db.Posts.FindAsync(postId));
            if (post == null)
            {
                throw new PostNotFoundException($"Post with id '{postId}' is not found.");
            }

            return post;
        }

        private async Task<AccountModel> GetAccount(Guid accountId, CancellationToken cancellationToken)
        {
            return await _accountsCash.GetOrCreate(accountId,
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

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
    public class AddCommentHandler : AsyncRequestHandler<AddCommentRequest>
    {
        private readonly ApplicationContext _db;
        private readonly Cash<PostModel> _postsCash;

        public AddCommentHandler(ApplicationContext db, Cash<PostModel> postsCash)
        {
            _db = db;
            _postsCash = postsCash;
        }

        protected override async Task Handle(AddCommentRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Content))
            {
                throw new InvalidParameterException("Comment content can't be empty.");
            }

            var post = await _postsCash.GetOrCreate(request.PostId,
                async () => await _db.Posts.FindAsync(request.PostId));
            if (post == null)
            {
                throw new PostNotFoundException($"Post with id '{request.PostId}' is not found.");
            }

            var comment = new CommentModel
            {
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                PostId = post.Id,
                CreatorId = request.AccountId
            };
            await SaveComment(comment);
            await UpdatePost(post);
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

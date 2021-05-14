using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class AddCommentHandler : IRequestHandler<AddCommentRequest, AddCommentResponse>
    {
        private readonly ApplicationContext _db;

        public AddCommentHandler(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<AddCommentResponse> Handle(AddCommentRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Content))
            {
                //TODO: handle error
            }

            var post = await _db.Posts.FindAsync(new object[] { request.PostId }, cancellationToken);
            if (post == null)
            {
                //TODO: handle error
            }

            var account = await _db.Accounts.FindAsync(new object[] { request.AccountId }, cancellationToken);
            if (account == null)
            {
                //TODO: handle error
            }

            var comment = new CommentModel
            {
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                Post = post,
                Creator = account
            };

            _db.Comments.Add(comment);           
            await _db.SaveChangesAsync(cancellationToken);

            await UpdatePost(post);

            return new AddCommentResponse();
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

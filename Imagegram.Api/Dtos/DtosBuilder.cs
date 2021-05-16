using Imagegram.Api.Database.Models;
using System.Collections.Generic;
using System.Linq;

namespace Imagegram.Api.Dtos
{
    public static class DtosBuilder
    {
        public static PostDto Build(PostModel post, AccountModel account = null)
        {
            var result = new PostDto
            {
                Id = post.Id,
                Creator = Build(account ?? post.Creator),
                CreatedAt = post.CreatedAt,
                ImageUrl = post.ImageUrl,
                Comments = new List<CommentDto>()
            };            

            if (post.CommentLast != null)
            {
                result.Comments.Add(Build(post.CommentLast));
            }

            if (post.CommentBeforeLast != null)
            {
                result.Comments.Add(Build(post.CommentBeforeLast));
            }

            return result;
        }

        public static CommentDto Build(CommentModel comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                Creator = Build(comment.Creator),
                CreatedAt = comment.CreatedAt
            };
        }

        public static AccountDto Build(AccountModel account)
        {
            return new AccountDto
            {
                Id = account.Id,
                Name = account.Name
            };
        }

        public static List<PostDto> Build(List<PostModel> posts, AccountModel account = null)
        {
            return posts
                .Select(post => Build(post, account))
                .ToList();
        }        
    }
}

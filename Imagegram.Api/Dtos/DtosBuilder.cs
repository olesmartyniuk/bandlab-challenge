using Imagegram.Api.Database.Models;
using System.Collections.Generic;
using System.Linq;

namespace Imagegram.Api.Dtos
{
    public static class DtosBuilder
    {
        public static PostDto Build(PostModel post)
        {
            return new PostDto
            {
                Id = post.Id,
                Creator = Build(post.Creator),
                CreatedAt = post.CreatedAt,
                ImageUrl = post.ImageUrl,
                Comments = new List<CommentDto>
                {
                    Build(post.CommentLast),
                    Build(post.CommentBeforeLast),
                }
            };
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

        public static List<PostDto> Build(List<PostModel> posts)
        {
            return posts.Select(post => Build(post)).ToList();
        }
    }
}

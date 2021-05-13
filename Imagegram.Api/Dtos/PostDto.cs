using System;

namespace Imagegram.Api.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public AccountDto Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CommentsCount { get; set; }
        public CommentDto CommentLast { get; set; }
        public CommentDto CommentBeforeLast { get; set; }
    }
}

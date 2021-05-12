using System;

namespace Imagegram.Api.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AccountDto Creator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
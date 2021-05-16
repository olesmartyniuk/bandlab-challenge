using System;
using System.Collections.Generic;

namespace Imagegram.Api.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public AccountDto Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CommentDto> Comments { get; set; }    
    }
}

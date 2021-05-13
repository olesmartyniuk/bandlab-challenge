using System.Collections.Generic;

namespace Imagegram.Api.Dtos
{
    public class GetPostsResponse : PostDto
    {
        public List<PostDto> Posts { get; internal set; }
        public string Cursor { get; set; }
    }
}
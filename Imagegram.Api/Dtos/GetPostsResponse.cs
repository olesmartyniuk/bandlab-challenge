using System.Collections.Generic;

namespace Imagegram.Api.Dtos
{
    public class GetPostsResponse
    {
        public List<PostDto> Posts { get; set; }
        public string Cursor { get; set; }
    }
}
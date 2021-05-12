using Imagegram.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Imagegram.Api.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;

        public PostController(ILogger<PostController> logger)
        {
            _logger = logger;
        }

        [HttpPost("posts")]
        public ActionResult<PostDto> Create()
        {
            return Created("url", new PostDto
            {
                CreatedAt = DateTime.UtcNow,
                Creator = new AccountDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Account"
                },
                Id = 1
            });
        }

        [HttpGet("posts")]
        public ActionResult<IEquatable<PostDto>> Get([FromQuery] string cursor, [FromQuery] int limit = 100)
        {
            return Ok(new List<PostDto> 
            { 
                new PostDto 
                { 
                    CreatedAt = DateTime.UtcNow, Id = 1, ImageUrl = "http://image.com" 
                } 
            });
        }

        [HttpPost("posts/{postId}/comments")]
        public ActionResult<CommentDto> AddComment([FromRoute]int postId, [FromBody]AddCommentRequest request)
        {
            return Created("url", new CommentDto
            {
                CreatedAt = DateTime.UtcNow,
                Creator = new AccountDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Account"
                },
                Id = 1
            });
        }
    }
}

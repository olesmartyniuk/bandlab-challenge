using Imagegram.Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imagegram.Api.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IMediator _mediator;

        public PostController(ILogger<PostController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// The method creates post with existed image.
        /// </summary>
        /// <returns>Created post.</returns>
        /// <response code="201">Post was created.</response>
        /// <response code="400">Request has incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        [HttpPost("posts")]
        public ActionResult<CreatePostResponse> Create([FromBody] CreatePostRequest request)
        {
            var response = _mediator.Send(request);

            return Created("url", response);
        }

        /// <summary>
        /// The method returns posts with two last comments. Posts are sorted by number of comments.
        /// </summary>
        /// <returns>Array of posts and current cursor.</returns>
        /// <response code="200">Posts</response>
        /// <response code="400">Query parameters have incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        [HttpGet("posts")]
        public async Task<ActionResult<GetPostsResponse>> Get([FromQuery] string cursor, [FromQuery] int limit = 100)
        {
            var request = new GetPostsRequest
            {
                Cursor = cursor,
                Limit = limit
            };
            var response = await _mediator.Send(request);
            return Ok(response);                
        }

        /// <summary>
        /// The method add a comment for a post.
        /// </summary>
        /// <returns>Post with added comment.</returns>
        /// <response code="201">Comment was added.</response>
        /// <response code="400">Query parameters have incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        /// <response code="404">Post doesn't exist.</response> 
        [HttpPost("posts/{postId}/comments")]
        public async Task<ActionResult<AddCommentResponse>> AddComment([FromRoute] int postId, [FromBody] AddCommentRequest request)
        {
            var response = await _mediator.Send(request);
            return Created("url", response);
        }
    }
}

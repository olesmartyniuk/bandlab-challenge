using Imagegram.Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Imagegram.Api.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// The method creates post with image that is sent as a body.
        /// </summary>
        /// <remarks>
        /// As body you need to provide an image in .png, .jpg or .bmp formats.
        /// The exact format of the image will be discovered from the content.        
        /// </remarks>
        /// <returns>Created post.</returns>
        /// <response code="201">Post was created.</response>
        /// <response code="400">Image has incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        [HttpPost("posts")]
        public async Task<ActionResult<PostDto>> Create()
        {
            using var stream = await GetBodyStream();
            var accountId = GetAccountId();

            var request = new CreatePostRequest
            {
                ImageData = stream,
                AccountId = accountId
            };

            var response = await _mediator.Send(request);

            return Created($"posts/{response.Id}", response);
        }        

        /// <summary>
        /// The method returns posts with two last comments. Posts are sorted by number of comments.
        /// </summary>
        /// <returns>Array of posts and current cursor.</returns>
        /// <response code="200">Posts sorted by number of comments.</response>
        /// <response code="400">Query parameters have incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        [HttpGet("posts")]
        public async Task<ActionResult<GetPostsResponse>> Get([FromQuery] string cursor, [FromQuery] int limit = 100)
        {
            var accountId = GetAccountId();
            var request = new GetPostsRequest
            {
                AccountId = accountId,
                Cursor = cursor,
                Limit = limit
            };
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        /// <summary>
        /// The method adds a comment for a post.
        /// </summary>
        /// <returns>Created comment.</returns>
        /// <response code="201">Comment was added.</response>
        /// <response code="400">Query parameters have incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        /// <response code="404">Post doesn't exist.</response> 
        [HttpPost("posts/{postId}/comments")]
        public async Task<ActionResult> AddComment([FromRoute] int postId, [FromBody] AddCommentRequest request)
        {
            request.PostId = postId;
            request.AccountId = GetAccountId();

            var response = await _mediator.Send(request);
            return Created($"posts/{postId}/comments/{response.Id}", response);
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(HttpContext
                .User
                .FindFirstValue(ClaimTypes.Authentication));
        }

        private async Task<MemoryStream> GetBodyStream()
        {
            HttpContext.Request.EnableBuffering();
            HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            var stream = new MemoryStream();
            await HttpContext.Request.Body.CopyToAsync(stream);
            stream.Position = 0;
            return stream;
        }
    }
}

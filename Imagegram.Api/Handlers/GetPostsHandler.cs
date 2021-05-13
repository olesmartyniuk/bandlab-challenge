using Imagegram.Api.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class GetPostsHandler : IRequestHandler<GetPostsRequest, GetPostsResponse>
    {
        public Task<GetPostsResponse> Handle(GetPostsRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetPostsResponse
            {
                Posts = new List<PostDto>
                {
                    new PostDto
                    {
                        CreatedAt = DateTime.UtcNow, Id = 1, ImageUrl = "http://image.com"
                    }
                },
                Cursor = "next"
            });
        }
    }
}

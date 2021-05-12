using Imagegram.Api.Dtos;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class CreatePostHandler : IRequestHandler<CreatePostRequest, CreatePostResponse>
    {
        public Task<CreatePostResponse> Handle(CreatePostRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

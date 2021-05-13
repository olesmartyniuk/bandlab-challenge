using Imagegram.Api.Dtos;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class AddCommentHandler : IRequestHandler<AddCommentRequest, AddCommentResponse>
    {
        public Task<AddCommentResponse> Handle(AddCommentRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AddCommentResponse
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

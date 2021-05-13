using MediatR;

namespace Imagegram.Api.Dtos
{
    public class AddCommentRequest : IRequest<AddCommentResponse>
    {
        public string Content { get; set; }
    }
}
using MediatR;

namespace Imagegram.Api.Dtos
{
    public class GetPostsRequest : IRequest<GetPostsResponse>
    {
        public string Cursor { get; internal set; }
        public int Limit { get; internal set; }
    }
}
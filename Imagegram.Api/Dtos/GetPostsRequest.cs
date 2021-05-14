using MediatR;
using System;

namespace Imagegram.Api.Dtos
{
    public class GetPostsRequest : IRequest<GetPostsResponse>
    {
        public Guid AccountId { get; set; }
        public string Cursor { get; internal set; }
        public int Limit { get; internal set; }
    }
}
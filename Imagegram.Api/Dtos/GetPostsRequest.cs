using MediatR;
using System;

namespace Imagegram.Api.Dtos
{
    public class GetPostsRequest : IRequest<GetPostsResponse>
    {
        public Guid AccountId { get; set; }
        public string Cursor { get; set; }
        public int Limit { get; set; }
    }
}
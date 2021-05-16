using MediatR;
using System;
using System.IO;

namespace Imagegram.Api.Dtos
{
    public class CreatePostRequest : IRequest<PostDto>
    {
        public Guid AccountId { get; set; }
        public MemoryStream ImageData { get; set; }
    }
}
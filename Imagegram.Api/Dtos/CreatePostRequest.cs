using MediatR;
using System;
using System.IO;

namespace Imagegram.Api.Dtos
{
    public class CreatePostRequest : IRequest<CreatePostResponse>
    {
        public Guid AccountId { get; set; }
        public MemoryStream ImageStream { get; set; }
    }
}
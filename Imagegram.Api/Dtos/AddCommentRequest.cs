using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Imagegram.Api.Dtos
{
    public class AddCommentRequest : IRequest<AddCommentResponse>
    {
        [Required]
        public string Content { get; set; }
        public int PostId { get; set; }        
        public Guid AccountId { get; internal set; }
    }
}
using MediatR;
using System;

namespace Imagegram.Api.Dtos
{
    public class DeleteAccountRequest : IRequest<DeleteAccountResponse>
    {
        public Guid AccountId { get; set; }
    }
}
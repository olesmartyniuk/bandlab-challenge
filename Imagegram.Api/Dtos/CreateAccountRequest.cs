using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Imagegram.Api.Dtos
{
    public class CreateAccountRequest : IRequest<AccountDto>
    {
        [Required]
        public string Name { get; set; }
    }
}
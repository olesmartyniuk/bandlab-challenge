using MediatR;

namespace Imagegram.Api.Dtos
{
    public class CreateAccountRequest : IRequest<AccountDto>
    {
        public string Name { get; set; }
    }
}
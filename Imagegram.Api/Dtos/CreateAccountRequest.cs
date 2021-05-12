using MediatR;

namespace Imagegram.Api.Dtos
{
    public class CreateAccountRequest : IRequest<CreateAccountResponse>
    {
        public string Name { get; set; }
    }
}
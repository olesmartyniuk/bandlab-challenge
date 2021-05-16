using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountRequest, AccountDto>
    {
        private readonly ApplicationContext _db;

        public CreateAccountHandler(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<AccountDto> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var account = new AccountModel
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(cancellationToken);

            return DtosBuilder.Build(account);
        }
    }
}

using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Imagegram.Api.Dtos;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountRequest, CreateAccountResponse>
    {
        private readonly ApplicationContext _db;

        public CreateAccountHandler(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<CreateAccountResponse> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var accountModel = new AccountModel
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.Accounts.Add(accountModel);
            await _db.SaveChangesAsync();

            return new CreateAccountResponse
            {
                Id = accountModel.Id,
                Name = accountModel.Name
            };
        }
    }
}

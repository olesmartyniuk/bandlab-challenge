using Imagegram.Api.Database;
using Imagegram.Api.Dtos;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Imagegram.Api.Handlers
{
    public class DeleteAccountHandler : IRequestHandler<DeleteAccountRequest, DeleteAccountResponse>
    {
        private readonly ApplicationContext _db;

        public DeleteAccountHandler(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<DeleteAccountResponse> Handle(DeleteAccountRequest request, CancellationToken cancellationToken)
        {
            var accountModel = await _db.Accounts.FindAsync(request.AccountId);
            _db.Accounts.Remove(accountModel);
            await _db.SaveChangesAsync();

            return new DeleteAccountResponse();
        }
    }
}

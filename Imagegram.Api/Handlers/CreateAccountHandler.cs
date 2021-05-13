using AutoMapper;
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
        private readonly IMapper _mapper;

        public CreateAccountHandler(ApplicationContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CreateAccountResponse> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var account = new AccountModel
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();

            return _mapper.Map<CreateAccountResponse>(account);
        }
    }
}

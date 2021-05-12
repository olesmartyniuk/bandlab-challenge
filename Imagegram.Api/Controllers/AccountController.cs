using Imagegram.Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Imagegram.Api.Controllers
{
    [ApiController]    
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMediator _mediator;

        public AccountController(ILogger<AccountController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("accounts")]
        public ActionResult<CreateAccountResponse> Create([FromBody] CreateAccountRequest request)
        {
            var response = _mediator.Send(request);
            return Created("accounts/1", response);
        }

        [HttpDelete("accounts/{accountId}")]
        public ActionResult Delete(Guid accountId)
        {
            var response = _mediator.Send(new DeleteAccountRequest
            {
                AccountId = Guid.NewGuid()
            });
            return Created("accounts/1", response);
        }
    }
}

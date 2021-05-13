using Imagegram.Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Imagegram.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMediator _mediator;

        public AccountController(ILogger<AccountController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// The method creates account in the system.
        /// </summary>
        /// <returns>Created account.</returns>
        /// <response code="201">Account was created.</response>
        /// <response code="400">Request has incorrect format.</response>         
        [HttpPost("accounts")]
        public async Task<ActionResult<CreateAccountResponse>> Create([FromBody] CreateAccountRequest request)
        {
            var response = _mediator.Send(request);
            return Created("accounts/1", response);
        }

        /// <summary>
        /// The method deletes account in the system with all related posts, images and comments.
        /// </summary>
        /// <returns>Created account.</returns>
        /// <response code="200">Account was deleted.</response>
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        [HttpDelete("accounts/{accountId}")]
        public async Task<ActionResult> Delete()
        {
            await _mediator.Send(new DeleteAccountRequest
            {
                AccountId = Guid.NewGuid()
            });
            return Ok();
        }
    }
}

using Imagegram.Api.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Imagegram.Api.Controllers
{
    [ApiController]    
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost("accounts")]
        public ActionResult<AccountDto> Create([FromBody] CreateAccountRequest request)
        {
            return Created("accounts/1", new AccountDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Account"
            });
        }

        [HttpDelete("accounts/{accountId}")]
        public ActionResult Delete(Guid accountId)
        {
            return Ok();
        }
    }
}

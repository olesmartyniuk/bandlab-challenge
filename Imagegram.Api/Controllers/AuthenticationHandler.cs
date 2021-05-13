using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Imagegram.Api.Database;
using Imagegram.Api.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Imagegram.Api.Controllers
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string AccountIdHeader = "X-Account-Id";
        private readonly ApplicationContext _db;

        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApplicationContext db)
            : base(options, logger, encoder, clock)
        {
            _db = db;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return AuthenticateResult.NoResult();
            }

            Guid accountId;
            if (!TryGetAccountId(out accountId))
            {
                return AuthenticateResult.Fail($"Missed {AccountIdHeader} header.");
            }

            var account = await FindAccount(accountId);
            if (account == null)
            {
                return AuthenticateResult.Fail("Invalid account.");
            }
            
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Name),                
                new Claim(ClaimTypes.Authentication,account.Id.ToString()),                
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private bool TryGetAccountId(out Guid accountId)
        {
            if (Request.Headers.TryGetValue(AccountIdHeader, out var headerValue))
            {                
                if (Guid.TryParse(headerValue.ToString(), out accountId))
                {
                    return true;
                }
            }

            accountId = default(Guid);
            return false;
        }

        private async Task<AccountModel> FindAccount(Guid accountId)
        {
            var accessToken = await _db.Accounts                
                .SingleOrDefaultAsync(a => a.Id == accountId);            
            
            return accessToken;
        }
    }
}
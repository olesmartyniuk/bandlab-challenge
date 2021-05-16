using FluentAssertions;
using Imagegram.Api.Dtos;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Imagegram.Api.Tests
{
    public class CreateAccountTests : BaseControllerTests
    {
        private const string AccountName = "Account 1";

        [Fact]
        public async Task AccountShouldBeCreatedSuccessfully()
        {
            var request = new CreateAccountRequest
            {
                Name = AccountName
            };

            var response = await Client.PostJson("accounts", request);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var payload = await response.GetBody<AccountDto>();
            payload.Id.Should().NotBeEmpty();
            payload.Name.Should().Be(AccountName);

            var db = ConnectToDatabase();
            var accounts = db.Accounts.ToList();
            accounts.Count.Should().Be(1);
            accounts.Should().Contain(ac => ac.Id == payload.Id && payload.Name == AccountName);
        }

        [Fact]
        public async Task AccountShouldNotBeCreatedIfBadRequest()
        {           
            var response = await Client.PostJson("accounts", "NOT A JSON");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var db = ConnectToDatabase();
            var accounts = db.Accounts.ToList();
            accounts.Count.Should().Be(0);
        }
    }
}

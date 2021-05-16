using System;
using System.Net.Http;
using Hellang.Middleware.ProblemDetails;
using Imagegram.Api.Authentication;
using Imagegram.Api.Controllers;
using Imagegram.Api.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Imagegram.Api.Tests
{
    public abstract class BaseControllerTests : BaseServiceTests
    {
        protected HttpClient Client { get; private set; }

        public BaseControllerTests()
        {
            // Run for every test case
            SetupServer();
        }

        private void SetupServer()
        {
            var builder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    //webHost.UseEnvironment("Production");
                    webHost.UseTestServer();
                    webHost.ConfigureTestServices(services =>
                    {
                        services.AddProblemDetails(ExceptionToStatusCodeMapper.Map);

                        AddApplicationServices(services);

                        services.AddAuthentication("HeaderAuthentication")
                            .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("HeaderAuthentication", null);
                        services.AddAuthorization();
                        services.AddControllers(options => {
                            options.Filters.Add(typeof(ModelStateValidator));
                        }).AddApplicationPart(typeof(Program).Assembly);                        

                        services.Configure<KestrelServerOptions>(options =>
                        {
                            options.AllowSynchronousIO = true;
                        });
                    });

                    webHost.Configure(app =>
                    {
                        app.UseProblemDetails();
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });

            var host = builder.Start();
            var testServer = host.GetTestServer();
            testServer.AllowSynchronousIO = true;
            Client = testServer.CreateClient();
        }
    }
}

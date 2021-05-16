using Imagegram.Api.Database.Models;
using Imagegram.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Imagegram.Api.Tests
{
    public abstract class BaseServiceTests : BaseDatabaseTests
    {
        private IServiceProvider _serviceProvider;

        protected readonly IConfiguration _configuration;
        protected readonly Mock<DateTimeService> _dateTimeService;
        protected readonly Mock<FileService> _fileService;

        public BaseServiceTests()
        {
            var services = new ServiceCollection();
            AddApplicationServices(services);

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"InMemoryCashMaxItems", "1024"}
                }).Build();
            _fileService = new Mock<FileService>(MockBehavior.Strict, _configuration);
            _dateTimeService = new Mock<DateTimeService>(MockBehavior.Strict);

            SetupBaseMocks();
        }

        private void SetupBaseMocks()
        {
        }

        protected void AddApplicationServices(IServiceCollection services)
        {
            services.AddScoped(sp => GetDatabase());
            services.AddScoped(sp => _configuration);

            services.AddSingleton(sp => _dateTimeService.Object);
            services.AddSingleton(sp => _fileService.Object);
            services.AddSingleton<Cash<AccountModel>>();
            services.AddSingleton<Cash<PostModel>>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMediatR(Assembly.GetAssembly(typeof(Startup)));

            _serviceProvider = services.BuildServiceProvider();
        }

        protected T GetSut<T>()
        {
            return (T)_serviceProvider.GetService(typeof(T));
        }
    }
}

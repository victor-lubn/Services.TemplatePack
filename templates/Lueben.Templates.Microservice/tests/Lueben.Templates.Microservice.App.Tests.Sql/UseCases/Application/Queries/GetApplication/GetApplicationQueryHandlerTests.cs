using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.Tests.Sql.Helpers;
using Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetApplication;
using Lueben.Templates.Microservice.Data.Sql;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Sql.UseCases.Application.Queries.GetApplication
{
    public class GetApplicationQueryHandlerTests
    {
        private readonly string _applicationId = Guid.NewGuid().ToString();
        private readonly IServiceCollection _serviceCollection;

        public GetApplicationQueryHandlerTests()
        {
            _serviceCollection = SetupServiceProvider();
        }

        [Fact]
        public async Task GivenGetApplicationQuery_WhenQueryIsExecuted_ThenTheApplicationShouldBeReturned()
        {
            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var mockGetCommand = new GetApplicationQuerySql
                {
                    Id = _applicationId
                };

                var mediator = provider.GetService<IMediator>();
                var application = await mediator.Send<GetApplicationQuerySql, Domain.Entities.Application>(mockGetCommand);

                Assert.NotNull(application);
                Assert.NotEqual(Guid.Empty.ToString(), application.Id);
            }
        }

        [Fact]
        public async Task GivenGetApplicationQuery_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var mockGetAllCommand = new GetApplicationQuerySql
                {
                    Id = Guid.NewGuid().ToString()
                };

                var mediator = provider.GetService<IMediator>();

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => mediator.Send<GetApplicationQuerySql, Domain.Entities.Application>(mockGetAllCommand));

                Assert.Equal("Entity not found.", exception.Message);
            }
        }

        private IServiceCollection SetupServiceProvider()
        {
            var context = MockContext();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly);
            serviceCollection.AddSingleton<ILuebenContextSql, LuebenContext>(x => context);
            serviceCollection.AddTransient<IMediator, Mediator>();
            serviceCollection.AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider));
            serviceCollection.AddTransient<IRequestHandler<GetApplicationQuerySql, Domain.Entities.Application>, GetApplicationQueryHandlerSql>();

            return serviceCollection;
        }

        private LuebenContext MockContext()
        {
            var context = LuebenDbContextFactory.CreateInMemory();

            var application = new Domain.Entities.Application
            {
                Id = _applicationId,
                PreferredDepotId = "1",
                Created = DateTime.UtcNow
            };

            context.Application.AddRange(new List<Domain.Entities.Application> { application });

            context.SaveChanges();

            return context;
        }
    }
}
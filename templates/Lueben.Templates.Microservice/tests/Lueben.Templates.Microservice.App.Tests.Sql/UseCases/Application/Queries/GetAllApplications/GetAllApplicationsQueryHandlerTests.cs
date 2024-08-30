using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.Tests.Sql.Helpers;
using Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetAllApplications;
using Lueben.Templates.Microservice.Data;
using Lueben.Templates.Microservice.Data.Sql;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Sql.UseCases.Application.Queries.GetAllApplications
{
    public class GetApplicationQueryHandlerTests
    {
        private readonly IServiceCollection _serviceCollection;

        public GetApplicationQueryHandlerTests()
        {
            _serviceCollection = SetupServiceProvider();
        }

        [Fact]
        public async Task GivenGetAllApplicationsQuery_WhenQueryIsExecuted_ThenListOfAllEntitiesShouldBeReturned()
        {
            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var mockGetAllCommand = new GetAllApplicationsQuerySql();

                var mediator = provider.GetService<IMediator>();
                var result = await mediator.Send<GetAllApplicationsQuerySql, IQueryable<Domain.Entities.Application>>(mockGetAllCommand);

                Assert.NotNull(result);
            }
        }

        private static IServiceCollection SetupServiceProvider()
        {
            var context = MockContext();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly);
            serviceCollection.AddSingleton<ILuebenContextSql, LuebenContext>(x => context);
            serviceCollection.AddTransient<IMediator, Mediator>();
            serviceCollection.AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider));
            serviceCollection.AddTransient<IRequestHandler<GetAllApplicationsQuerySql, IQueryable<Domain.Entities.Application>>, GetAllApplicationsQueryHandlerSql>();

            return serviceCollection;
        }

        private static LuebenContext MockContext()
        {
            var context = LuebenDbContextFactory.CreateInMemory();

            var application1 = new Domain.Entities.Application
            {
                Id = Guid.NewGuid().ToString(),
                PreferredDepotId = "1",
                Created = DateTime.UtcNow
            };

            var application2 = new Domain.Entities.Application
            {
                Id = Guid.NewGuid().ToString(),
                PreferredDepotId = "3",
                Created = DateTime.UtcNow
            };

            context.Application.AddRange(new List<Domain.Entities.Application> { application1, application2 });

            return context;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetAllApplications;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Cosmos.UseCases.Application.Queries.GetAllApplications
{
    public class GetApplicationCosmosQueryHandlerTests
    {
        [Fact]
        public async Task GivenGetAllApplicationsQuery_WhenQueryIsExecuted_ThenListOfAllEntitiesShouldBeReturned()
        {
            var mockApplication = new List<Domain.Entities.Application>
            {
                new Domain.Entities.Application()
                {
                    Id = Guid.NewGuid().ToString()
                }
            };

            var mockRepository = new Mock<IRepositoryCosmos<Domain.Entities.Application>>();
            mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Application, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<IEnumerable<Domain.Entities.Application>>(Task.FromResult(mockApplication.AsEnumerable())));

            var serviceCollection = SetupServiceProvider(mockRepository);
            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mockGetAllCommand = new GetAllApplicationsQueryCosmos();

                var mediator = provider.GetService<IMediator>();
                var result = await mediator.Send<GetAllApplicationsQueryCosmos, IQueryable<Domain.Entities.Application>>(mockGetAllCommand);

                Assert.NotNull(result);

                mockRepository.Verify(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Application, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        private static IServiceCollection SetupServiceProvider(Mock<IRepositoryCosmos<Domain.Entities.Application>> mockRepository)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly)
                             .AddTransient<IMediator, Mediator>()
                             .AddTransient(provider => mockRepository.Object)
                             .AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider))
                             .AddTransient<IRequestHandler<GetAllApplicationsQueryCosmos, IQueryable<Domain.Entities.Application>>, GetAllApplicationsQueryHandlerCosmos>();

            return serviceCollection;
        }
    }
}

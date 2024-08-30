using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetApplication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Cosmos.UseCases.Application.Queries.GetApplication
{
    public class GetApplicationCosmosQueryHandlerTests
    {
        [Fact]
        public async Task GivenGetApplicationQuery_WhenQueryIsExecuted_ThenTheApplicationShouldBeReturned()
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
                var mockGetCommand = new GetApplicationQueryCosmos
                {
                    Id = Guid.NewGuid().ToString()
                };

                var mediator = provider.GetService<IMediator>();
                var application = await mediator.Send<GetApplicationQueryCosmos, Domain.Entities.Application>(mockGetCommand);

                Assert.NotNull(application);
                Assert.NotEqual("", application.Id);

                mockRepository.Verify(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Application, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenGetApplicationQuery_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            var mockApplication = new List<Domain.Entities.Application>();

            var mockRepository = new Mock<IRepositoryCosmos<Domain.Entities.Application>>();
            mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Application, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<IEnumerable<Domain.Entities.Application>>(Task.FromResult(mockApplication.AsEnumerable())));

            var serviceCollection = SetupServiceProvider(mockRepository);
            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mockGetAllCommand = new GetApplicationQueryCosmos
                {
                    Id = Guid.NewGuid().ToString()
                };

                var mediator = provider.GetService<IMediator>();

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => mediator.Send<GetApplicationQueryCosmos, Domain.Entities.Application>(mockGetAllCommand));

                Assert.Equal("Entity not found.", exception.Message);

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
                             .AddTransient<IRequestHandler<GetApplicationQueryCosmos, Domain.Entities.Application>, GetApplicationQueryHandlerCosmos>();

            return serviceCollection;
        }
    }
}

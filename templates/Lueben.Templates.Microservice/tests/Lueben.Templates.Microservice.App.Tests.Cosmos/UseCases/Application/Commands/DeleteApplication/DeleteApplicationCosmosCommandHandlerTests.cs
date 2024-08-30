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
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.DeleteApplication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Cosmos.UseCases.Application.Commands.DeleteApplication
{
    public class DeleteApplicationCosmosCommandHandlerTests
    {
        [Fact]
        public async Task GivenDeleteApplicationCommand_WhenCommandIsExecuted_ThenEntityAndAllTheChildrenShouldBeDeletedFromTheDatabase()
        {
            var mockDeleteCommand = new DeleteApplicationCommandCosmos
            {
                Id = Guid.NewGuid().ToString()
            };

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
            mockRepository.Setup(x => x.DeleteAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<CancellationToken>()))
                .Callback<Domain.Entities.Application, CancellationToken>((app, token) =>
                {
                    Assert.NotNull(app);
                    Assert.Equal(app.Id, mockDeleteCommand.Id);
                })
                .Returns(new ValueTask());

            var serviceCollection = SetupServiceProvider(mockRepository);

            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mediator = provider.GetService<IMediator>();
                await mediator.Send<DeleteApplicationCommandCosmos, Unit>(mockDeleteCommand);

                mockRepository.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenDeleteApplicationQuery_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            var mockApplication = new List<Domain.Entities.Application>();

            var mockRepository = new Mock<IRepositoryCosmos<Domain.Entities.Application>>();
            mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Application, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<IEnumerable<Domain.Entities.Application>>(Task.FromResult(mockApplication.AsEnumerable())));

            var serviceCollection = SetupServiceProvider(mockRepository);

            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mockDeleteCommand = new DeleteApplicationCommandCosmos
                {
                    Id = Guid.NewGuid().ToString()
                };

                var mediator = provider.GetService<IMediator>();

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => mediator.Send<DeleteApplicationCommandCosmos, Unit>(mockDeleteCommand));

                Assert.Equal("Entity not found.", exception.Message);

                mockRepository.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        private static IServiceCollection SetupServiceProvider(Mock<IRepositoryCosmos<Domain.Entities.Application>> mockRepository)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly)
                             .AddTransient<IMediator, Mediator>()
                             .AddTransient(provider => mockRepository.Object)
                             .AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider))
                             .AddTransient<IRequestHandler<DeleteApplicationCommandCosmos, Unit>, DeleteApplicationCommandHandlerCosmos>();

            return serviceCollection;
        }
    }
}
